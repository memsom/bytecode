		ORG START 
		NOP
		NOP                               
		NOP
		NOP                               		                             
START:  ST_S 'This is a test program.',13,10,$  
		TRP 21                                  
		ST_S 'Call a sub routine',13,10,$       
		TRP 21                                  
		JSR SUB1                                
		ST_S 'Returned from sub 1.',13,10,$       
		TRP 21   
		NOP
		NOP                               
		JMP ENDP  	                             
SUB1:   ST_S 'In sub routine',13,10,$           
		TRP 21 
		JSR SUB2 
		ST_S 'Returned from sub 2.',13,10,$       
		TRP 21                                 
		RSR 
SUB2:   ST_S 'In sub routine 2, from sub 1',13,10,$           
		TRP 21   
		JSR SUB3 
		ST_S 'Returned from sub 3.',13,10,$       
		TRP 21                               
		RSR  
SUB3:   ST_S 'In sub routine 3, from sub 2',13,10,$           
		TRP 21 
		ST_B 0                                               
LOOP:	ST_B 10                                               
		ADD_B                  
		LD_B V1                                               
		SM_B V1                     
		SM_B V1                           
		SM_B V1                     
		TRP 21                         
		ST_S '',13,10,$                                  
		TRP 21                        
		CMP_B 50             
		JNE LOOP                                                
		ST_S 'Done',13,10,$  
		TRP 21                                     
		RSR  
		NOP
		NOP
		NOP                            
ENDP:   NOP
		ST_S 'Program end',13,10,$       
		TRP 21 
		END                                     


		