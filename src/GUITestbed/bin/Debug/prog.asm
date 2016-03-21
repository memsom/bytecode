        ORG START 
        NOP                              		                             
START:  ST_B 0          ; store byte value 0 on stack
        LD_B V1 ;  x    ; put value on stack in to the V1 register
        ST_B 0          ; store byte value 0 on stack
        LD_B V2 ;  y    ; put value on stack in to the V2 register
        NOP
LOOP:   SM_B V1         ; store V1 on stack
        SM_B V2         ; store V1 on stack
        ST_B 0          ; store 0 on stack
        TRP 25          ; call trap 25 (TRP_GUIDRAW) (this takes c, y, x from stack and calls CallGraphicsOutputEvent(...))
        NOP
        SM_B V1         ; store V1 on stack
        ST_B 1          ; store 1 on stack
        ADD_B           ; ADD last two values on stack, leaving the result on the stack
        LD_B V1         ; put stack value in V1
        NOP
        SM_B V2         ; store V2 on stack
        ST_B 1			; store 1 on stack
        ADD_B			; ADD last two values on stack, leaving the result on the stack
        LD_B V2			; put stack value in V2
        NOP
        SM_B V1
        CMP_B 255
        JNE LOOP        ; repeat
        NOP
STOP:   NOP
        END