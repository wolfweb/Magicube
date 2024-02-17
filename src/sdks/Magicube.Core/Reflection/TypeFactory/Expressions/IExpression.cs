using System;
using System.Reflection;
using Magicube.Core.Reflection;

namespace Magicube.Core.Reflection.Expressions {
    public interface IExpression {
        IExpression LdArg(int index);

        IExpression LdArgS(int index);

        IExpression LdArg0();

        IExpression LdArg1();

        IExpression LdArg2();

        IExpression LdArg3();

        T LdArg<T>(int index);

        T LdArgS<T>(int index);

        T LdArg0<T>();

        T LdArg1<T>();

        T LdArg2<T>();

        T LdArg3<T>();

        IExpression LdLoc(ILocal local);

        T LdLoc<T>(ILocal local);

        T LdLocS<T>(ILocal local);

        T LdLoc0<T>();

        T LdLoc1<T>();

        T LdLoc2<T>();

        T LdLoc3<T>();

        object LdNull();

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

        int LdcI4_M1();

        int LdcI4_S();

        long LdcI8(long value);

        float LdcR4(float value);

        double LdcR8(double value);

        T LdFld<T>(IFieldBuilder field);

        IExpression LdFld(IFieldBuilder field);

        T Value<T>();

        object Value(Type type);

        TResult Call<TResult>(MethodInfo methodInfo);
    }
}