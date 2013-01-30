using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace CSMSL.IO
{
    public class MzLite : IDisposable
    {        
                
        private static readonly string s_Synchrouns = "PRAGMA synchronous = @synchronous";
        private static readonly string s_TableExists = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
        private static readonly string s_Connection = "Data Source={0};Version=3;";

        private SQLiteCommand insertSpectraSQL = null;
        private SQLiteCommand assocpeptoprotSQL = null;
        private SQLiteCommand insertPeptideSQL = null;
        private SQLiteCommand insertProteinSQL = null;        

        public enum Synchronicity { Off = 0, Normal = 1, Full = 2 }

        private SQLiteConnection m_dbConnection;
        private SQLiteTransaction m_dbTransaction;

        public long LastInsertRowID
        {
            get { return m_dbConnection.LastInsertRowId; }
        }

        private Synchronicity m_synchronous = Synchronicity.Full;
        public Synchronicity Synchronous
        {
            get { return m_synchronous; }
            set
            {
                if (m_synchronous == value)
                    return;
                if (SetSynchronoity(value))
                {
                    m_synchronous = value;
                }                
            }
        }

        private string m_fileName;
        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        public MzLite(string fileName)
        {
            FileName = fileName;
        }

        public void Open()
        {
            if (!File.Exists(this.FileName))
                SQLiteConnection.CreateFile(this.FileName);
                        
            m_dbConnection = new SQLiteConnection(string.Format(s_Connection, this.FileName));
            m_dbConnection.Open();
            CreateBaseTables();
        }

        private void CreateBaseTables()
        {
            //try
            //{
                BeginTransaction();
                string sql = new StreamReader("Resources/mzlite_create.sqlite").ReadToEnd();
                SQLiteCommand createTable = new SQLiteCommand(sql, m_dbConnection);
                createTable.ExecuteNonQuery();           
                CommitTransaction();
            //}
            //catch (Exception)
            //{
            //    RollBackTransaction();
            //}
        }

        public void Close()
        {
            if (m_dbConnection == null)
                return;

            try
            {
                m_dbConnection.Close();
            }
            finally
            {
                Dispose();
            }            
        }
           
     
        private bool SetSynchronoity(Synchronicity synchronous)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(s_Synchrouns, m_dbConnection);
                switch(synchronous) {
                    default:
                    case Synchronicity.Full:
                        cmd.Parameters.AddWithValue("@synchronous", "FULL");
                        break;
                    case Synchronicity.Normal:
                        cmd.Parameters.AddWithValue("@synchronous", "NORMAL");
                        break;
                    case Synchronicity.Off:
                        cmd.Parameters.AddWithValue("@synchronous", "FULL");
                        break;
                }            
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a table exists in this database
        /// </summary>
        /// <param name="tableName">The name of the table to check for existence</param>
        /// <returns>True if the table exists, False otherwise</returns>
        public bool ContainsTable(string tableName)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(s_TableExists, m_dbConnection);
                cmd.Parameters.AddWithValue("@tableName", tableName);             
                return cmd.ExecuteReader().HasRows;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }
                
        /// <summary>
        /// Start a new transaction. Must be matched with either a CommitTranscation
        /// or RollBackTranscation method.
        /// </summary>
        public void BeginTransaction()
        {
            m_dbTransaction = m_dbConnection.BeginTransaction(); 
        }

        /// <summary>
        /// Commits the open transaction to the database file/memory.
        /// </summary>
        /// <returns>True if the commit is successful, False otherwise</returns>
        public bool CommitTransaction()
        {
            if (m_dbTransaction == null)
                return false;

            try
            {
                m_dbTransaction.Commit();
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
            finally
            {
                m_dbTransaction.Dispose();
            }
        }

        /// <summary>
        /// Roll backs the data in the database file/memory to the state 
        /// before the transaction was started.
        /// </summary>
        /// <returns>True if the rollback is successful, False otherwise</returns>
        public bool RollBackTransaction()
        {
            if (m_dbTransaction == null)
                return false;

            try
            {
                m_dbTransaction.Rollback();
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
            finally
            {
                m_dbTransaction.Dispose();
            }
        }

        public object ExecuteScalar(string format, params object[] args)
        {
            return ExecuteScalar(string.Format(format, args));
        }

        public object ExecuteScalar(string sql)
        {
            if (m_dbConnection == null)
                return null;

            SQLiteCommand cmd = m_dbConnection.CreateCommand();
            cmd.CommandText = sql;
            return cmd.ExecuteScalar();     
        }

        public int ExecuteNonQuery(string format, params object[] args)
        {
            return ExecuteNonQuery(string.Format(format, args));
        }

        public int ExecuteNonQuery(string sql)
        {
            if(m_dbConnection == null)
                return 0;

            SQLiteCommand cmd = m_dbConnection.CreateCommand();
            cmd.CommandText = sql;            
            return cmd.ExecuteNonQuery();         
        }

        /// <summary>
        /// Insert a peptide sequence into the database
        /// </summary>
        /// <param name="sequence">The peptide amino acid sequence</param>
        /// <returns>The peptide ID for the database</returns>
        public long InsertPeptide(string sequence)
        {
            string peptide = sequence.ToUpper();
            double mass = CSMSL.Proteomics.AminoAcidPolymer.GetMass(peptide);

            if (insertPeptideSQL == null)
            {
                insertPeptideSQL = new SQLiteCommand("INSERT INTO peptides(sequence, mass) VALUES(@sequence, @mass)", m_dbConnection);
            }

            insertPeptideSQL.Parameters.AddWithValue("@sequence", peptide);
            insertPeptideSQL.Parameters.AddWithValue("@mass", mass);
            insertPeptideSQL.ExecuteNonQuery();
            insertPeptideSQL.Parameters.Clear();
          
            long pepID = LastInsertRowID;
            if (pepID <= 0)
            {
                pepID = (long)ExecuteScalar("SELECT ID FROM peptides WHERE sequence='{0}'", peptide); 
            }
            return pepID;                  
        }

        private SQLiteCommand insertUniprotProteinSQL = null;

        /// <summary>
        /// Insert a protein into the database
        /// </summary>
        /// <param name="description">The description of the protein</param>
        /// <param name="sequence">The protein amino acid sequence</param>
        /// <returns>The protein ID for the database</returns>
        public long InsertProtein(string description, string sequence)
        {
            if (insertProteinSQL == null)
            {
                insertProteinSQL = new SQLiteCommand("INSERT INTO proteins(descriptionID, sequence, decoy) VALUES(@descriptionID, @sequence, @decoy)", m_dbConnection);
            }
            if (insertUniprotProteinSQL == null)
            {
                insertUniprotProteinSQL = new SQLiteCommand("INSERT INTO protein_headers(accession, entryName, proteinName,organismID, geneName, proteinExistence, sequenceVersion) VALUES(@accession, @entry, @protein, @organismID, @genename, @pe, @sv)", m_dbConnection);
            }

            sequence = sequence.ToUpper();
            //description = description.Replace("\'", "");
            Regex uniprot = new Regex(@"(?:(?:(DECOY)_)?)?(.+)\|(.+)\|(.+?)\s(.+?)\sOS=(.+?)(?:\sGN=(.+?))?(?:$|PE=(\d+)\sSV=(\d+))");

            Match match = uniprot.Match(description);
            if (match.Success)
            {
                string accession = match.Groups[3].Value;
                string entry = match.Groups[4].Value;
                string protein = match.Groups[5].Value;
                string organism = match.Groups[6].Value.Trim();

                ExecuteNonQuery("INSERT INTO organisms(scientific_name) VALUES('{0}')", organism);
                long organismID = (long)ExecuteScalar("SELECT ID FROM organisms WHERE scientific_name='{0}'",organism);

                string gn = match.Groups[7].Value;
                int pe = -1;
                int sv = -1;
                if (int.TryParse(match.Groups[8].Value, out pe))
                {
                    insertUniprotProteinSQL.Parameters.AddWithValue("@pe", pe);
                }
                else
                {
                    insertUniprotProteinSQL.Parameters.AddWithValue("@pe", null);
                }
                if (int.TryParse(match.Groups[9].Value, out sv))
                {
                    insertUniprotProteinSQL.Parameters.AddWithValue("@sv", sv);
                }
                else
                {
                    insertUniprotProteinSQL.Parameters.AddWithValue("@sv", null);
                }
                insertUniprotProteinSQL.Parameters.AddWithValue("@accession",accession);
                insertUniprotProteinSQL.Parameters.AddWithValue("@entry", entry);
                insertUniprotProteinSQL.Parameters.AddWithValue("@protein", protein);
                insertUniprotProteinSQL.Parameters.AddWithValue("@organismID", organismID);
                insertUniprotProteinSQL.Parameters.AddWithValue("@genename", gn);
                insertUniprotProteinSQL.ExecuteNonQuery();

                long descriptionID = (long)ExecuteScalar("SELECT ID FROM protein_headers WHERE accession='{0}'", accession);


                insertUniprotProteinSQL.Parameters.Clear();



                bool isDecoy = description.StartsWith("DECOY");
                insertProteinSQL.Parameters.AddWithValue("@descriptionID", descriptionID);
                insertProteinSQL.Parameters.AddWithValue("@sequence", sequence);
                insertProteinSQL.Parameters.AddWithValue("@decoy", isDecoy ? 1 : 0);
                insertProteinSQL.ExecuteNonQuery();
                insertProteinSQL.Parameters.Clear();

          

            }
            long protID = LastInsertRowID;
            if (protID <= 0)
            {
                protID = (long)ExecuteScalar("SELECT ID FROM proteins WHERE description='{0}'", description);
            }
            return protID;
        }


        public long InsertSpectra(int spectrumNumber, double retentionTime, long datafileID)
        {
            if (insertSpectraSQL == null)
            {
                insertSpectraSQL = new SQLiteCommand("INSERT INTO spectra(scan_number, retention_time, msdatafileID) VALUES(@spectrumNumber,@rt, @datafileID)", m_dbConnection);
            }
            insertSpectraSQL.Parameters.AddWithValue("@spectrumNumber", spectrumNumber);
            insertSpectraSQL.Parameters.AddWithValue("@datafileID", datafileID);
            insertSpectraSQL.Parameters.AddWithValue("@rt", retentionTime);
            insertSpectraSQL.ExecuteNonQuery();
            insertSpectraSQL.Parameters.Clear();

            long spectID = LastInsertRowID;
            if (spectID <= 0)
            {
                spectID = (long)ExecuteScalar("SELECT ID FROM spectra WHERE scan_number='{0}'", spectrumNumber);
            }
            return spectID;            
        }


        public int AssociatePeptideToProtein(long pepID, long protID, int startResidue, int stopResidue)
        {
            if (assocpeptoprotSQL == null)
            {
                assocpeptoprotSQL = new SQLiteCommand("INSERT INTO peptide_to_protein(peptideID, proteinID, startResidue, stopResidue) VALUES(@pepID, @protID, @startResidue, @stopResidue)", m_dbConnection);
            }
            assocpeptoprotSQL.Parameters.AddWithValue("@pepID", pepID);
            assocpeptoprotSQL.Parameters.AddWithValue("@protID", protID);
            assocpeptoprotSQL.Parameters.AddWithValue("@startResidue", startResidue);
            assocpeptoprotSQL.Parameters.AddWithValue("@stopResidue", stopResidue);
            int result = assocpeptoprotSQL.ExecuteNonQuery();
            assocpeptoprotSQL.Parameters.Clear();
            return result;         
        }

        private SQLiteCommand assocpeptospecSQL = null;

        public int AssociatePeptideToSpectrum(long pepID, long spectrumID)
        {
            if (assocpeptospecSQL == null)
            {
                assocpeptospecSQL = new SQLiteCommand("INSERT INTO peptide_spectral_matches(peptideID, spectraID) VALUES(@pepID, @spectraID)", m_dbConnection);
            }
            assocpeptospecSQL.Parameters.AddWithValue("@pepID", pepID);
            assocpeptospecSQL.Parameters.AddWithValue("@spectraID", spectrumID);
            int result = assocpeptospecSQL.ExecuteNonQuery();
            assocpeptospecSQL.Parameters.Clear();
            return result;
        }
        
        public void Dispose()
        {
            if (m_dbConnection != null)
            {
                m_dbConnection.Dispose();
            }
        }
    }
}
