using System;
using System.Text;
using HtmlAgilityPack;

namespace DIP.Shared
{
    public class HtmlParser
    {
        public static string GetCleanedText(string  text)
        {
            if(text.IsNullOrEmpty()) return text;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);

            HtmlNode node = doc.DocumentNode;
           return GetCleanedTextByNode(node);
        }

        static string GetCleanedTextByNode(HtmlNode node)
        {
            // If the node is a text node, return its text
            if (node.NodeType == HtmlNodeType.Text)
            {
                return node.InnerText + "   ";
            }

            // If the node is an element node, recursively process its child nodes
            StringBuilder sb = new StringBuilder();
            foreach (HtmlNode childNode in node.ChildNodes)
            {
                sb.Append(GetCleanedTextByNode(childNode));
            }
            return sb.ToString();
        }
    }
}
