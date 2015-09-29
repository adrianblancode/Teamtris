# Teamtris
Cooperative tetris using Unity and wiimotes. The two players share a block and control it from their own 2D perspective, together they must navigate a 3D playing field.

[adrianblan.co/Teamtris](http://adrianblan.co/Teamtris/)

## How to build
Clone the repository using ``git clone https://github.com/adrianblp/Teamtris.git``. 

Open Unity and select "Other Project", navigate to where you saved the repository and select the "Teamtris" folder.

### Wiimotes

By default, wiimotes are not enabled. In order to enable them you must first connect two wiimotes via bluetooth to your computer, download GlovePIE and run the script WiiOSC.PIE inside the "GlovePie Scripts" folder. Lastly edit the BlockController.cs files to change ENABLE_WIIMOTE to true.

##Images

![Teamtris image](http://i.imgur.com/qpJ1KfL.jpg)

## Contributors
Teamtris was developed by Adrian Blanco, Douglas Carlsson, Emilie Le Moël, Victor Hung and Mårten Norman for the course Advanced Graphics and Interaction at KTH.
