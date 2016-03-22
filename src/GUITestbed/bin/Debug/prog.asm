        ORG START                               		                             
START:  ST_B 10         ; store byte value 0 on stack
        LD_B V1 ;  x    ; put value on stack in to the V1 register
        ST_B 10         ; store byte value 0 on stack
        LD_B V2 ;  y    ; put value on stack in to the V2 register        
        JSR PROG        
        JMP STOP
PROG:   JSR PLOT      
        RSR
PLOT:   SM_B V1         ; store V1 on stack
        SM_B V2         ; store V1 on stack
        ST_B 0          ; store 0 on stack
        TRP 25          ; call trap 25 (TRP_GUIDRAW) (this takes c, y, x from stack and calls CallGraphicsOutputEvent(...))
        RSR       
STOP:   END