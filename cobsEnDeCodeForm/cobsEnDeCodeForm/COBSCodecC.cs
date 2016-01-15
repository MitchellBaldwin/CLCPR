using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WindowsFormsApplication1
{
    class COBSCodecC
    {


        public unsafe void StuffData(byte *ptr, ulong length, byte *dst)
        {
        byte *end = ptr + length;
        byte *code_ptr = dst++;
        byte code = 0x01;
        
            while (ptr < end)
            {
            if (*ptr == 0) 
            {
               // FinishBlock(code);
                 *code_ptr = (code); 
                 code_ptr = dst++;
                 code = 0x01;
            }
            else
                {
                *dst++ = *ptr;
                code++;
                
                if (code == 0xFF) 
                    {
                    //FinishBlock(code);

                    *code_ptr = (code); 
                    code_ptr = dst++;
                    code = 0x01;
                    }
                }
            ptr++;
            }
        //FinishBlock(code);
        *code_ptr = (code); 
        code_ptr = dst++;
        code = 0x01;

        }


       public unsafe void UnStuffData(byte *ptr,ulong length, byte *dst)
        {
        byte *end = ptr + length;
        
           while (ptr < end)
            {
            int i, code = *ptr++;
            for (i=1; i<code; i++) *dst++ = *ptr++;
                
            if (code < 0xFF) *dst++ = 0;
            }
        }

       
      //  unsafe FinishBlock(X) (*code_ptr = (X), code_ptr = dst++, code = 0x01)

    }
}
