# workclone(C#) 

[![Last Commit](https://img.shields.io/github/last-commit/ffishstix/workcloneCS)](https://github.com/ffishstix/workcloneCS)
[![Discord](https://img.shields.io/badge/Discord-fish__stix-7289DA?logo=discord&logoColor=white)](https://discord.gg/BgCp6FMxTN)


put simply a clone of a very popular pos system...

---
## order of operations
- to begin with the program will run a sync connecting to my personally hosted server with some default credentials that can only read the database and shouldnt be able to change anything as i havent gotten to the point where i need to push orders yet
- if all of the data isnt null and has been able to be gathered successfully either with backup files or database connections then it will continue and show the main window
- in the main window you have so so many choice:
---
### if you choose:
- any catagory, the said items should show up with each items prefered colour (havent implemented chosen order yet although i could)
- any item, the item should get added to the current sale board and can be edited further
- button originally labed standard, that is the pricing option and can be used to modify every item selected however it isnt implemented
- Name button, it takes you to a second menu asking for you to input your pin which returns the credentials and is used when pushing the food to the table (when i finally implement it)
- Table, eventually i will add this functionality but i get the feeling it is going to take quite a while for it to work out as there is a lot that will go into it and wont be as simple as preseting some tables and will think of the manager side of it 
- on the bottom row:
- Misc + Table + Final + Config, they bring up generic menus and it is pretty self explanatory explore it yourself it honestly isnt that dificult
- however Order is where shit gets interesting as there is a bit more (janky) logic that goes into it, basically it should allow you to be able to see more items i use it at work a lot so i wanted it to be well implemented spoiler it isnt but that is besides the point. the main thing to take away is that it gives you better control of the items and more visibility.
- back, pretty easily explained = you click on a catagory you click back and it goes back wow who would have thought!!!
---
## todo

- a lot more realistic:
    - [x] items displayed properly - that was very difficult, full refactor 
    - [x] reformat how items are stored in variables when clicked on (food items)
    - [x] give functionality for sign on and sign off buttons
    - [x] add first run form to initialize database connections
    - [x] add responsive errors to the first run form so the user knows what to change
    - [x] add tab so i can easily input data
    - [x] add ability for users to go back to last successsful login data


- a couple packages that might be nice to integrate:
  - [ ] MaterialSkin - recomended for nicer visuals
  - [x] FluentValidation - sped up validation significantly (ai written)
  - [ ] dependency injection - it was suggested and its probs a good idea tbf


- bit far out:
  - [ ] add the ordering system
  - [ ] including the databases to go along with it
  - [ ] add a quick website to view the ordering system
  - [ ] add allergies database with a connected info button to each item with an allergy connected 
  - [ ] make an admin form where you can set prices for different products this could be by pulling all items from the database looking at it through a list then selecting the one you want and at the backend it makes the SQL code tbf could be an idea 
  - [ ] with the last option add standard deviation and mean of prices so that you can get extra marks for it 


---
please help me with this page i am absolutely useless at markdown (and everything else tbh) and if you have absolutely any ideas or help for it i would appreciate it so much i dont think you understand but yeah contact me with a quick dm i am open for any ideas ;)