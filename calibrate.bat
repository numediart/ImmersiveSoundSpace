:: © - 2020 – UMONS - CLICK' Living Lab
:: 
:: Immersive Sound Space of University of MONS – CLICK' Living Lab (Fabien Grisard) is free software: 
:: you can redistribute it and/or modify it under the terms of the 3-Clause BSD licence. 
:: This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
:: without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
:: See the 3-Clause BSD licence License for more details.
:: 
:: ​​​​​You should have received a copy of the 3-Clause BSD licence along with this program.  
:: 
:: Each use of this software must be attributed to University of MONS – CLICK' Living Lab  (Fabien Grisard).

@echo off

:: set anaconda environment for the python script
call activate iss

python Tracking/HTCTrackerPositionSender.py --origin_serial LHR-08B8C6A7 --framerate 25
