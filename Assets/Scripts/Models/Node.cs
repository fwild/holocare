using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Node
    {
        /// <summary>
        /// The current node is a question, but doesn't have any predefined answers
        /// </summary>
        public bool IsFreestyleQuestion;
        public string Content;
        public List<Node> Children;
        public Node ParentNode;

        public string SelectedAnswer;

        public bool IsAnswer { get { return Children == null && !IsFreestyleQuestion; } }


        /// <summary>
        /// Should be used from the root node
        /// </summary>
        /// <param name="_index">0 is the most left Question</param>
        /// <returns></returns>
        public Node GetQuestionByIndex(int _index)
        {
            Node currentNode = MostLeftQuestion();
            int currentIndex = 0;

            if (_index == 0)
                return currentNode;

            while (currentIndex != _index)
            {
                int nodeIndexInParent = currentNode.ParentNode == null ? -1 : currentNode.ParentNode.Children.IndexOf(currentNode);
                //We stay within the same depth
                if (nodeIndexInParent < currentNode.ParentNode.Children.Count - 1)
                {
                    nodeIndexInParent++;
                    currentNode = currentNode.ParentNode.Children[nodeIndexInParent].MostLeftQuestion();

                    currentIndex++;
                }
                //We keep jumping to the top
                else
                {
                    //while (currentNode.)
                }
            }

            return currentNode;
        }
        Node MostLeftQuestion()
        {
            if (Children != null)
                return Children[0].MostLeftQuestion();

            if (IsAnswer)
                return ParentNode;

            return this;
        }
    }
}
