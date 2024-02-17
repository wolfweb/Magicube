namespace Magicube.Core.Reflection.Expressions {
    public interface IIterator {
        IIterator LdLoc(ILocal local);

        IIterator LdLoc0();

        IIterator LdLoc1();

        IIterator LdLoc2();

        IIterator LdLoc3();

        IIterator StLoc(ILocal local);

        IIterator StLoc0();

        IIterator StLoc1();

        IIterator StLoc2();

        IIterator StLoc3();

        IIterator LdcI4(int value);

        IIterator LdcI4_0();

        IIterator LdcI4_1();

        IIterator LdcI4_2();

        IIterator LdcI4_3();

        IIterator LdcI4_4();

        IIterator LdcI4_5();

        IIterator LdcI4_6();

        IIterator LdcI4_7();

        IIterator LdcI4_8();

        IIterator LdcI4_M1();

        IIterator LdcI8(int value);

        IIterator LdcR4(float value);

        IIterator LdcR8(double value);

        IIterator Add();

        IIterator AddOvf();

        IIterator AddOvfUn();

        IIterator Sub();

        IIterator SubOvf();

        IIterator SubOvfUn();

        IIterator Div();

        IIterator DivUn();

        IIterator Mul();

        IIterator MulUn();

        IIterator Inc(ILocal local);

        IIterator Dec(ILocal local);
    }
}