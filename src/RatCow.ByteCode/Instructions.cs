using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RatCow.ByteCode
{
    public enum Instructions
    {
        NOP = 0,    //NO OPERATION
        
        ORG = 1,    //ORIGIN
        END = 2,    //END  of code
        
        LD_B = 3,   //LOAD BYTE
        LD_W = 13,  //     WORD
        LD_S = 23,  //     STRIBNG
        
        ST_B = 4,   //STORE BYTE
        ST_W = 14,  //      WORD
        ST_S = 24,  //      STRING

        STD_S = 34, //      ????
        
        SM_B = 5,   //ST REG BYTE   
        SM_W = 15,  //       WORD
        SM_S = 25,  //       STRING
    
        DM_S = 6,   //define a memory location with a string value 
        DM_B = 16,
        DM_W = 26,
        
        TRP = 10,   //TRAP
        
        ADD_B = 11, //ADD BYTE
        ADD_W = 21,

        SUB_B = 27, //SUBTRACT BYTE
        SUB_W = 37,

        MUL_B = 28, //Multiply BYTE
        MUL_W = 38,

        DIV_B = 12, //Divide BYTE
        DIV_W = 22,
        DIVF_B = 62, //Divide BYTE
        DIVF_W = 82,
        MOD_B = 63, //modulus BYTE
        MOD_W = 83,

        JSR = 40,   //JUMP TO SUB ROUTINE
        RSR = 41,   //RETURN FROM SUB ROUTINE
        
        JMP = 42,   //UNCONDITIONAL JUMP
        JE = 43,    //JUMP IF EQUAL
        JNE = 44,   //JUMP IF  NOT EQUAL
        
        CMP_B = 50, //COMPARE BYTE
        CMP_W = 60, //COMPARE BYTE

        LBL = 300,  //LABEL
    }

    public enum Registers
    {
        V1 = 254,
        V2 = 253,
    }
}
