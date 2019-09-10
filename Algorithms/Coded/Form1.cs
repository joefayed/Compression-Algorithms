using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coded
{
    public partial class Form1 : Form
    {
        int min = 5, max = 5000, step = 1000, nor = 3;
        double benchmarktime1 = 0;
        double benchmarktime2 = 0;
        string input;
        string w = string.Empty;
        List<int> compressed = new List<int>();
        List<int> compressed2 = new List<int>();
        List<int> compressedbench = new List<int>();
        StringBuilder decompressed;
        StringBuilder inputbench;
        Random rnd = new Random();
        BitArray encoded;
        HuffmanTree huffmanTree;
        public class Node
        {
            public char Symbol { get; set; }
            public int Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }

            public List<bool> Traverse(char symbol, List<bool> data)
            {
                // Leaf
                if (Right == null && Left == null)
                {
                    if (symbol.Equals(this.Symbol))
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (Left != null)
                    {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(data);
                        leftPath.Add(false);

                        left = Left.Traverse(symbol, leftPath);
                    }

                    if (Right != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(data);
                        rightPath.Add(true);
                        right = Right.Traverse(symbol, rightPath);
                    }

                    if (left != null)
                    {
                        return left;
                    }
                    else
                    {
                        return right;
                    }
                }
            }
        }

        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node Root { get; set; }
            public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

            public void Build(string source)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (!Frequencies.ContainsKey(source[i]))
                    {
                        Frequencies.Add(source[i], 0);
                    }

                    Frequencies[source[i]]++;
                }

                foreach (KeyValuePair<char, int> symbol in Frequencies)
                {
                    nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                    if (orderedNodes.Count >= 2)
                    {
                        // Take first two items
                        List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                        // Create a parent node by combining the frequencies
                        Node parent = new Node()
                        {
                            Symbol = '*',
                            Frequency = taken[0].Frequency + taken[1].Frequency,
                            Left = taken[0],
                            Right = taken[1]
                        };

                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }

                    this.Root = nodes.FirstOrDefault();

                }

            }

            public BitArray Encode(string source)
            {
                List<bool> encodedSource = new List<bool>();

                for (int i = 0; i < source.Length; i++)
                {
                    List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                    encodedSource.AddRange(encodedSymbol);
                }

                BitArray bits = new BitArray(encodedSource.ToArray());

                return bits;
            }

            public string Decode(BitArray bits)
            {
                Node current = this.Root;
                string decoded = "";

                foreach (bool bit in bits)
                {
                    if (bit)
                    {
                        if (current.Right != null)
                        {
                            current = current.Right;
                        }
                    }
                    else
                    {
                        if (current.Left != null)
                        {
                            current = current.Left;
                        }
                    }

                    if (IsLeaf(current))
                    {
                        decoded += current.Symbol;
                        current = this.Root;
                    }
                }

                return decoded;
            }

            public bool IsLeaf(Node node)
            {
                return (node.Left == null && node.Right == null);
            }

        }

        public void compress(string inputs)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString(), i);
            }
            foreach (char c in inputs)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    compressed.Add(dictionary[w]);
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
            if (!string.IsNullOrEmpty(w))
            {
                compressed.Add(dictionary[w]);
            }
        }

        public void decompress()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(i, ((char)i).ToString());
            }
            string ww = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            decompressed = new StringBuilder(w);
            foreach (int c in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(c))
                {
                    entry = dictionary[c];
                }
                else if (c == dictionary.Count)
                {
                    entry = w + w[0];
                }
                decompressed.Append(entry);
                dictionary.Add(dictionary.Count, w + entry[0]);
                w = entry;
            }
        }

        public void decompress2(string inputs)
        {
            Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
            {
                dictionary2.Add(i, ((char)i).ToString());
            }
            char t = ',';
            string[] temp = inputs.Split(t);
            for (int i = 0; i < temp.Length; i++)
            {
                int no = Int32.Parse(temp[i]);
                compressed2.Add(no);
            }
            string ww = dictionary2[compressed2[0]];
            compressed2.RemoveAt(0);
            decompressed = new StringBuilder(w);
            foreach (int c in compressed2)
            {
                string entry = null;
                if (dictionary2.ContainsKey(c))
                {
                    entry = dictionary2[c];
                }
                else if (c == dictionary2.Count)
                {
                    entry = w + w[0];
                }
                decompressed.Append(entry);
                dictionary2.Add(dictionary2.Count, w + entry[0]);
                w = entry;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void generatingbench(int no)
        {
            inputbench = new StringBuilder("k");
            for (int i = 0; i < no - 1; i++)
            {
                int z = rnd.Next(0, 255);
                inputbench.Append(((char)z).ToString());
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox6.Text == " ")
            {
                MessageBox.Show("Please Enter A Text To Compress");
            }
            else
            {
                input = richTextBox6.Text;
                huffmanTree = new HuffmanTree();
                huffmanTree.Build(input);
                encoded = huffmanTree.Encode(input);
                richTextBox3.Visible = true;
                for (int i = 0; i < encoded.Count; i++)
                {
                    richTextBox3.Text += (encoded[i] ? 1 : 0).ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox6.Text == " ")
            {
                MessageBox.Show("Please Enter A Text To Compress");
            }
            else
            {
                string decoded = huffmanTree.Decode(encoded);
                richTextBox4.Visible = true;
                richTextBox4.Text = decoded;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();
            var watch2 = new System.Diagnostics.Stopwatch();
            for (int s = min; s < max; s += step)
            {
                generatingbench(s);
                BitArray temp = null;
                for (int i = 0; i < nor; i++)
                {
                    watch.Restart();
                    huffmanTree = new HuffmanTree();
                    huffmanTree.Build(inputbench.ToString());
                    BitArray encoded2 = huffmanTree.Encode(inputbench.ToString());
                    benchmarktime1 += watch.Elapsed.TotalMilliseconds;
                    if (i == 0)
                    {
                        temp = huffmanTree.Encode(inputbench.ToString());
                    }
                    watch.Stop();

                }
                //==================================================================================================
                for (int i = 0; i < nor; i++)
                {
                    watch2.Restart();
                    string decoded = huffmanTree.Decode(temp);
                    benchmarktime2 += watch2.Elapsed.TotalMilliseconds;
                    watch2.Stop();
                }
                if (s == min)
                {
                    this.richTextBox5.Text = "Data Size \t" + "Compress time \t" + "Decompress Time \t" + "\n" + "=====================================================" + "\n";
                }
                this.richTextBox5.Text += s + "\t\t" + Math.Round((benchmarktime1 / nor), 3).ToString() + "\t\t" + Math.Round((benchmarktime2 / nor), 3).ToString() + "\n";
                benchmarktime1 = 0;
                benchmarktime2 = 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == " ")
            {
                MessageBox.Show("Please Enter A Text To Compress");
            }
            else
            {
                compressed.Clear();
                input = richTextBox1.Text;
                compress(input);
                richTextBox8.Visible = true;
                for (int i = 0; i < compressed.Count; i++)
                {
                    richTextBox8.Text += compressed[i].ToString() + ", ";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == " " && richTextBox2.Text == " ")
            {
                MessageBox.Show("Please Enter A Text To Compress first or enter a decompressed array");
            }
            else if (richTextBox2.Text != " ")
            {
                input = richTextBox2.Text;
                decompress2(input);
                richTextBox9.Visible = true;
                richTextBox9.Text = decompressed.ToString();
            }
            else if (richTextBox2.Text == " ")
            {
                input = richTextBox1.Text;
                compress(input);
                decompress();
                richTextBox9.Visible = true;
                richTextBox9.Text = decompressed.ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();
            var watch2 = new System.Diagnostics.Stopwatch();
            for (int s = min; s < max; s += step)
            {
                generatingbench(s);
                compressed.Clear();
                for (int i = 0; i < nor; i++)
                {
                    watch.Restart();
                    compress(inputbench.ToString());
                    benchmarktime1 += watch.Elapsed.TotalMilliseconds;
                    if (i == 0)
                    {
                        compressedbench = compressed;
                    }
                    watch.Stop();

                }
                //==================================================================================================
                compressed = compressedbench;
                for (int i = 0; i < nor; i++)
                {
                    watch2.Restart();
                    decompress();
                    benchmarktime2 += watch2.Elapsed.TotalMilliseconds;
                    watch2.Stop();
                }
                if (s == min)
                {
                    this.richTextBox12.Text = "Data Size \t" + "Compress time \t" + "Decompress Time \t" + "\n" + "========================================================" + "\n";
                }
                this.richTextBox12.Text += s + "\t\t" + Math.Round((benchmarktime1 / nor), 3).ToString() + "\t\t" + Math.Round((benchmarktime2 / nor), 3).ToString() + "\n";
                benchmarktime1 = 0;
                benchmarktime2 = 0;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (richTextBox7.Text == " ")
            {
                MessageBox.Show("Please Enter A Text To Compress");
            }
            else
            {
                input = richTextBox7.Text;
                huffmanTree = new HuffmanTree();
                huffmanTree.Build(input);
                encoded = huffmanTree.Encode(input);
                richTextBox10.Visible = true;
                for (int i = 0; i < encoded.Count; i++)
                {
                    richTextBox10.Text += (encoded[i] ? 1 : 0).ToString();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (richTextBox7.Text == " ")
            {
                MessageBox.Show("Please Enter A Text To Compress");
            }
            else
            {
                string decoded = huffmanTree.Decode(encoded);
                richTextBox11.Visible = true;
                richTextBox11.Text = decoded;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();
            var watch2 = new System.Diagnostics.Stopwatch();
            for (int s = min; s < max; s += step)
            {
                generatingbench(s);
                BitArray temp = null;
                for (int i = 0; i < nor; i++)
                {
                    watch.Restart();
                    compress(inputbench.ToString());
                    huffmanTree = new HuffmanTree();
                    huffmanTree.Build(compressed.ToString());
                    BitArray encoded2 = huffmanTree.Encode(compressed.ToString());
                    benchmarktime1 += watch.Elapsed.TotalMilliseconds;
                    if (i == 0)
                    {
                        temp = huffmanTree.Encode(compressed.ToString());
                    }
                    watch.Stop();

                }
                //==================================================================================================
                for (int i = 0; i < nor; i++)
                {
                    watch2.Restart();
                    decompress();
                    string decoded = huffmanTree.Decode(temp);
                    benchmarktime2 += watch2.Elapsed.TotalMilliseconds;
                    watch2.Stop();
                }
                if (s == min)
                {
                    this.richTextBox13.Text = "Data Size \t" + "Compress time \t" + "Decompress Time \t" + "\n" + "=====================================================" + "\n";
                }
                this.richTextBox13.Text += s + "\t\t" + Math.Round((benchmarktime1 / nor), 3).ToString() + "\t\t" + Math.Round((benchmarktime2 / nor), 3).ToString() + "\n";
                benchmarktime1 = 0;
                benchmarktime2 = 0;
            }
        }
    }
}
