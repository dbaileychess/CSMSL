using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Util.Collections
{
    public class BinaryTree<T>
    {
        private BinaryTreeNode<T> _rootNode;
        private int _count;

        public int Count { get { return _count; } }

        public BinaryTree()
        {
            _count = 0;
        }


    }
}
