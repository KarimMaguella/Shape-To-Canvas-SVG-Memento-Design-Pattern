using System;
using System.Collections.Generic;
using System.IO;

namespace Memento
{
    //Using the Memento Design Pattern approach for this assignment...
    class Program
    {
        public static int WIDTH = 1450;
        public static int HEIGHT = 850;

        //This method generates a file with the SVG format of every Shape stored in the canvas!
        public static void genFile(String filename, Stack<Memento> canvas)
        {
            String path = @".\OutputFiles\" + filename;

            if (canvas.Count != 0)
            {
                Console.WriteLine("CREATING XML FILE...");
                string svgOpen = String.Format(@"<svg height=""{0}"" width=""{1}"" xmlns=""http://www.w3.org/2000/svg"">" + Environment.NewLine, HEIGHT, WIDTH);
                string svgClose = Environment.NewLine + @"</svg>";

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(svgOpen);

                    //This is where you pop a queue of shapes from the canvas, until queue is empty
                    while (canvas.Count != 0)
                    {
                        sw.WriteLine(canvas.Pop().savedShape.dispSVG + Environment.NewLine);
                    }
                    sw.WriteLine(svgClose);
                }
                Console.WriteLine("FILE CREATED!!!");
            }
            else if (!path.Substring(path.Length - 4).ToLower().Equals(".xml"))
            {
                Console.WriteLine("EXTENSION IS EITHER NOT INCLUDED OR NOT xml");
            }
            else
            {
                Console.WriteLine("FILE ALREADY EXISTS IN PATH: " + path);
            }
        }


        public class Memento
        {
            public Shape savedShape;
            Stack<Memento> redo = new Stack<Memento>();
            private Memento(Shape shape)
            {
                this.savedShape = shape;
            }

            public class Originator
            {
                //Shapes s is our state
                public Shape s;
                //Keeping track of state using stacks
                public Stack<Memento> redo = new Stack<Memento>();

                public void Set(Shape shape)
                {
                    Console.WriteLine("Originator setting Shape to: " + shape);
                    this.s = shape;
                }

                //Save new state
                public Memento addToMemento()
                {
                    Console.WriteLine("Originator: Saving to Memento");
                    return new Memento(s);
                }
                //Undo operation by popping the stack<memento> through this function
                public void undoFromMemento(Memento m)
                {
                    //must be ready to Redo operation so keep track of that momento for later..
                    this.redo.Push(m);
                    this.s = m.savedShape;
                    Console.WriteLine("Originator: undo operation from Memento with shape: " + s);
                }
                //Redo operation we already our saved states in a stack, simply pop and update..
                public Memento redoMemento()
                {
                    Memento m = this.redo.Pop();
                    this.s = m.savedShape;
                    Console.WriteLine("Originator: Redoing operation from Memento with shape: " + s);
                    return m;
                }
            }
        }

        //Caretaker class takes care of the canvas/history/order of operations + has the main so everything else..
        class Caretaker
        {
            public static Random random()
            {
                Random rnd = new Random();
                return rnd;
            }
            static void Main(string[] args)
            {
                Stack<Memento> canvas = new Stack<Memento>();
                Memento.Originator originator = new Memento.Originator();

                Random rnd = new Random();

                //Shape Examples
                originator.Set(new Rectangle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ));
                canvas.Push(originator.addToMemento());

                originator.Set(new Square( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ));
                canvas.Push(originator.addToMemento());

                originator.Set(new Circle(rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500)));
                canvas.Push(originator.addToMemento());

                //originator.undoFromMemento(canvas.Pop());

                string help = "*********************************************************\n\tCommands:\n\n\tH \t\t Help -displays this message\n\tA <shape>\t Add <shape to canvas>\n\tU\t\t Undo last operation\n \tR \t\t Redo last operation\n \tP \t\t Print Canvas\n \tC \t\t Clear canvas \n \tG <filename> \t Generate Output File in XML\n \tQ \t\t Quit application\n\n\tShapes => [C = Circle, S = Square, R = Rectangle]\n*********************************************************";
                Console.WriteLine(help);

                string input = Console.ReadLine().ToLower();
                while (!input.StartsWith('q'))
                {
                    if (input.StartsWith('h'))
                    {
                        Console.WriteLine(help);
                    }
                    //Add a Shape
                    else if (input.StartsWith('a'))
                    {
                        string[] inputArray = input.Split(' ', StringSplitOptions.TrimEntries);
                        string inputShape = inputArray[1].ToLower();

                        Console.WriteLine(inputShape);
                        //Add Circle By User Input
                        if (inputShape.StartsWith('c'))
                        {
                            originator.Set(new Circle(random().Next(1, 500), random().Next(1, 500), random().Next(1, 500)));
                            canvas.Push(originator.addToMemento());
                        }
                        //Add Rectangle By User Input
                        else if (inputShape.StartsWith('r'))
                        {
                            originator.Set(new Rectangle( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500)));
                            canvas.Push(originator.addToMemento());
                        }
                        else if (inputShape.StartsWith('s'))
                        {
                            originator.Set(new Square( rnd.Next(1, 500), rnd.Next(1, 500), rnd.Next(1, 500) ));
                            canvas.Push(originator.addToMemento());
                        }
                    }
                    else if (input.StartsWith('u'))
                    {
                        Memento temp = canvas.Pop();
                        originator.undoFromMemento(temp);
                    }
                    else if (input.StartsWith('r'))
                    {
                        canvas.Push(originator.redoMemento());
                    }
                    else if (input.StartsWith('p'))
                    {
                        foreach (Memento i in canvas.ToArray())
                        {
                            Console.WriteLine(i.savedShape.ToString());
                        }
                    }
                    else if (input.StartsWith('c'))
                    {
                        while(canvas.Count > 0)
                        {
                            Console.WriteLine("Clearing " + canvas.Pop().savedShape.ToString());
                        }
                        Console.WriteLine("Console has been cleared");
                    }
                    else if (input.StartsWith('g'))
                    {
                        string[] inputArray = input.Split(' ', StringSplitOptions.TrimEntries);
                        string fileName = inputArray[1].ToLower();

                        genFile(fileName + ".xml", canvas);
                    }
                    input = Console.ReadLine().ToLower();
                }

                Console.WriteLine();
                Console.WriteLine("Dont't forget to check out any output files you may have created in './OutputFiles' dir");
                Console.WriteLine();

                Console.WriteLine("GoodBye!");
                Console.WriteLine();
            }
        }

        // Abstract Shape class
        public abstract class Shape
        {
            virtual public String dispSVG { get; set; }
            public override string ToString()
            {
                return "Shape!";
            }
        }

        // Square Shape class
        public class Square : Shape
        {
            public int Length { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }
            override public String dispSVG { get; set; }

            public Square(int len, int x, int y)
            {
                Length = len;
                X = x;
                Y = y;

                dispSVG = String.Format(@"<rect width=""{0}"" height=""{0}"" x=""{1}"" y=""{2}"" stroke=""purple"" stroke-width=""20"" fill=""cyan"" />", Length, X, Y);
            }

            public override string ToString()
            {
                return "Square [x: " + X + ", y: " + Y + ", length: " + Length + "]";
            }
        }


        // Circle Shape class
        public class Circle : Shape
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int R { get; private set; }
            override public String dispSVG { get; set; }

            public Circle(int x, int y, int r)
            {
                X = x; Y = y; R = r;
                dispSVG = String.Format(@"<circle cx=""{0}"" cy=""{1}"" r=""{2}"" stroke=""black"" stroke-width=""2"" fill=""purple"" />", X, Y, R);
            }

            public override string ToString()
            {
                return "Circle [x: " + X + ", y: " + Y + ", r: " + R + "]";
            }
        }

        public class Rectangle : Shape
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }
            override public String dispSVG { get; set; }

            public Rectangle(int w, int h, int x, int y)
            {
                Width = w;
                Height = h;
                X = x;
                Y = y;

                dispSVG = String.Format(@"<rect width=""{0}"" height=""{1}"" x=""{2}"" y=""{3}"" stroke=""purple"" stroke-width=""20"" fill=""cyan"" />", Width, Height, X, Y);
            }

            public override string ToString()
            {
                return "Rectangle [Width: " + Width + ", Height: " + Height + ", X: " + X + ", Y: " + Y + "]";
            }
        }
    }
}