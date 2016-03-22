		ORG START 
		NOP
		NOP                               
		NOP
		NOP                               		                             
START:  ST_S 'Program start :: testing maths op codes.',13,10,$  
		TRP 21 
	    JSR MULB
		JSR DIVB
		JSR MODB
		JSR DIVFB
		JMP ENDP
		NOP
MULB:	ST_S '5 * 2 = ',$  
		TRP 21
		ST_B 5
		ST_B 2
		MUL_B
		TRP 21 
		ST_S '',13,10,$   
		TRP 21  
		RSR                             
		NOP
DIVB:	ST_S '5 / 6 = ',$  
		TRP 21
		ST_B 5
		ST_B 2
		DIV_B
		TRP 21 
		ST_S '',13,10,$   
		TRP 21  
		RSR                             
		NOP
MODB:	ST_S '5 % 2 = ',$  
		TRP 21
		ST_B 5
		ST_B 2
		MOD_B
		TRP 21 
		ST_S '',13,10,$   
		TRP 21  
		RSR                             
		NOP
DIVFB:	ST_S '5 / 2 = ',$  
		TRP 21
		ST_B 5
		ST_B 2
		DIVF_B
		TRP 21 
		ST_S '',13,10,$   
		TRP 21  
		RSR                             
		NOP
		NOP                            
ENDP:   NOP
		ST_S 'Program end',13,10,$       
		TRP 21 
		END                                     


		