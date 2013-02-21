using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Util.Collections
{
    public class BinaryTreeNode<T>
    {
        public BinaryTreeNode<T> LeftChild { get; private set; }
        public BinaryTreeNode<T> RightChild { get; private set; }
        public T Value { get; set; }
        public bool IsLeafNode { get { return LeftChild == null && RightChild == null; } }

        public BinaryTreeNode(T value, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
        {
            Value = value;
            LeftChild = left;
            RightChild = right;
        }

        public BinaryTreeNode(T value)
            : this(value, null, null) { }
    }
}
