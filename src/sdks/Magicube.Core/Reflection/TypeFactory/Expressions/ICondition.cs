namespace Magicube.Core.Reflection.Expressions {
    public interface ICondition {
        T LdLoc<T>(ILocal local);

        T LdLoc0<T>();

        T LdLoc1<T>();

        T LdLoc2<T>();

        T LdLoc3<T>();

        int LdcI4(int value);

        int LdcI4_0();

        int LdcI4_1();

        int LdcI4_2();

        int LdcI4_3();

        int LdcI4_4();

        int LdcI4_5();

        int LdcI4_6();

        int LdcI4_7();

        int LdcI4_8();
    }
}