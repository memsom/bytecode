		ORG START
START:  ST_B 10                                               
LOOP:	ST_B 10                                               
		ADD_B    ;;value will go back on stack                
		LD_B V1                                               
		SM_B V1  ;;value we use next loop                     
		SM_B V1  ;;value we compare                           
		SM_B V1  ;;value we echo to console                   
		TRP 21   ;;writes to the console                      
		ST_S '',13,10,$ ;;13,10,0                                  
		TRP 21   ;;writes to the console                      
		CMP_B 50 ;;compares stack to the constant             
		JNE LOOP                                                
		ST_S 'The End',13,10,$ ;;84,104,101,32,69,110,100,13,10,0  
		TRP 21   ;;writes to the console                      
		END 
		;;000, 002, 004, 010, 004, 010, 006, 002, 254, 022, 254, 022, 254, 022, 254, 010, 021, 012, 013, 010, 000, 010, 021, 009, 050, 019, 004, 012, 084, 104, 101, 032, 069, 110, 100, 013, 010, 000, 010, 021, 001                                                  
