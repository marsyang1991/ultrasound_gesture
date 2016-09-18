using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    class Complex
    {
        private double real;
        private double image;

        public double Real
        {
            get{return real;}
            set{real = value;}
        }
        public double Image
        {
            get{return image;}
            set{image = value;}
        }
        public Complex(double real,double image)
        {
            this.image = image;
            this.real = real;
        }
        public Complex()
            : this(0,0){ }
        public Complex(double real)
            :this(real,0){ }
        //加法
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Real + c2.Real, c1.Image + c2.Image);
        }
        //减法
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Real - c2.Real, c1.Image - c2.Image);
        }
        //乘法
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Real * c2.Real - c1.Image * c2.Image, c1.Real * c2.Image + c1.Image * c2.Real);
        }
        //取模
        public double ToModul()
        {
            return Math.Sqrt(real * real + image * image);
        }
        public override string ToString()
        {
            if (Real == 0 && Image == 0)
            {
                return string.Format("{0}", 0);
            }
            if (Real == 0 && (Image != 1 && Image != -1))
            {
                return string.Format("{0}i", Image);
            }
            if (Real == 0&&Image==1)
            {
                return string.Format("i");
            }
            if (Real == 0 && Image == -1)
            {
                return string.Format("-i");
            }
            if (Image < 0)
            {
                return string.Format("{0}-{1}i", Real, -Image);
            }
            return string.Format("{0}+{1}i", Real, Image);
        }
    }
}
