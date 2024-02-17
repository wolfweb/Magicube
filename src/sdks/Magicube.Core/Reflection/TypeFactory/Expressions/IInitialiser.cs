namespace Magicube.Core.Reflection.Expressions {
    public interface IInitialiser {
        IInitialiser DeclareLocal<T>(out ILocal local);

        IInitialiser DeclareLocal<T>(string localName, out ILocal local);

        IInitialiser LdLoc(ILocal local);

        IInitialiser StLoc(ILocal local);

        IInitialiser LdcI4(int value);

        IInitialiser LdcI4_0();

        IInitialiser LdcI4_1();

        IInitialiser LdcI4_2();

        IInitialiser LdcI4_3();

        IInitialiser LdcI4_4();

        IInitialiser LdcI4_5();

        IInitialiser LdcI4_6();

        IInitialiser LdcI4_7();

        IInitialiser LdcI4_8();

        IInitialiser LdcI4_M1();

        IInitialiser LdcI8(int value);

        IInitialiser LdcR4(float value);

        IInitialiser LdcR8(double value);
    }
}