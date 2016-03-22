		ORG START                               ;;00 2         0, 2
START:  ST_S 'This is a test program.',13,10,$  ;;02 26        12, 84, 104, 105, 115, 32, 105, 115, 32, 97,32,116,101,115,116,32,112,114,111,114,97,109,46,13,10,0 
		TRP 21                                  ;;29 2         10,21
		ST_S 'Call a sub routine',13,10,$       ;;30 21
		TRP 21                                  ;;51 2         10,21
		JSR SUB1                                ;;2  -- 57
		ST_S 'Returned from sub.',13,10,$       ;;22 -- 79
		TRP 21                                  ;;2  -- 81     10,21
		JMP ENDP                                ;;2  -- 84
SUB1:   ST_S 'In sub routine',13,10,$           ;;17 -- 101
		TRP 21                                  ;;2  -- 103    10,21
		RSR                                     ;;1  -- 104
ENDP:   END                                     ;;1  -- 105