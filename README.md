# MoleBlaster

## Information
This software was meant to be used to allow users to import a ChemDraw XML file.  Upon import of the chemDraw file, a third part library called "indigo.NET" is used to read in the chemical structures.  The software was never finished as the project unexpectantly halted.  If interest in continued development re-emerges, original author interested in collaborating to finish.

The point of this software was to be able to define a set of rules to fragment certain molecule classes.  The application is particularly useful with Ultraviolet Photodissociation (UVPD) wherein there are many pathways to fragmentation due to the high energy input into activation and investigation of molecules whose fragmentation pathways by UVPD are less known (e.x. lipids). 

Feel free to use this codebase as a starting project for your own work.

## General-Use
1. Import A Molecule "Template" where you use R-groups (generic groups)
   * You would typically treat R-groups as places where you would not expect cleavages.  
2. Turn template into Molecule by defining R-Groups.
3. Cleave the molecules in a "Smart" or "Naive" method
    * Smart = You define cleavage points, and what the resulting mass shift on each submolecule should be expected.
    * Naive = Attempts to cleave every combination of bonds.  Homolytic cleavage assumed.
4. Analyze results
    * Combinatorics calculations are performed and each sub-molecule/fragment monoisotopic mass is output.


## More information
Original development by W. Ryan Parker (https://github.com/wrparker) from the Brodbelt group at The University of Texas at Austin.
