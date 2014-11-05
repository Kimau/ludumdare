// Ludum Dare 25: You are the Villian by Claire Blackshaw (Kimau)
//                         
// [^ \n][ ]*\{[ ]*$
//------------------------------------------------------------------------
/*jshint multistr:true */
/*jshint sub:true */
/*jshint browser:true */

var m = new MersenneTwister();

// Game Resources
var playerName;
var opList;
var jobList;
var playerBank;

// Agent Stuff
var agentLevels = [ "Freshie" , "Street", "City", "National", "International", "Spy" ];
var skillTiers = [1, 2, 3, 5, 8, 13];
var nameList = ["Aada", "Aaron", "Adam", "Adam", "Adam", "Adam", "Adam", "Adam", "Adna", "Adnan", "Adéla", "Agnes", "Ahmed", "Aiden", "Aikaterini", "Aikaterini", "Aimar", "Aina", "Aino", "Aisha", "Ajdin", "Alba", "Aleksa", "Aleksandar", "Aleksandra", "Aleksandra", "Aleksandre", "Aleksandrs", "Aleksi", "Alessandro", "Alessandro", "Alessia", "Alex", "Alex", "Alex", "Alexa", "Alexander", "Alexander", "Alexander", "Alexandru", "Alexey", "Alfie", "Alfie", "Alfie", "Alice", "Alice", "Alice", "Alice", "Alice", "Alice", "Alicia", "Alikhan", "Alinami", "Alisa", "Alisa", "Alise", "Alisha", "Amanda", "Amber", "Amelia", "Amelia", "Amelia", "Amelia", "Amina", "Amir", "AmyAaliyah", "Ana", "Ana", "Ana", "Ana", "Ana", "Anahit", "Anastasija", "Anastasija", "Anastasiya", "Anastasiya", "Anastasiya", "Ander", "Andre", "Andrea", "Andrei", "Andrei", "Andrej", "Andrey", "Andria", "Ane", "Ane", "Angeliki", "Angelina", "Ani", "Ani", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anna", "Anni", "Anouk", "Anton", "Anton", "Anđela", "Anže", "Aputsiaq", "Aram", "Arbër", "Ardian", "Ardita", "Arinami", "Arjeta", "Arlind", "Arman", "Armandas", "Arnau", "Aron", "Arseny", "Arta", "Artem", "Artem", "Artem", "Artjoms", "Artur", "Artūrs", "Aruzhan", "Arzu", "Athanasios", "Aurel", "Aurora", "Austėja", "Ava", "Ava", "Ava", "Ava", "Axel", "Axelle", "Aya", "Ayan", "Ayar", "Ayauly", "Ayoub", "Ayzere", "Azamat", "Balázs", "Barbare", "Barbora", "Bartosz", "Basiliki", "Ben", "Bence", "Benjamin", "Benjamin", "Biel", "Biljana", "Blerina", "Blerta", "Bohdan", "Bram", "Burim", "Cameron", "Camille", "Camille", "Carla", "Carla", "Carolina", "Charlie", "Charlie", "Charlie", "Charlie", "Charlie", "Charlotte", "Chiara", "Chiara", "Chiara", "Chloé", "Chloé", "Chloé", "Chloé", "Chloé", "Christina", "Christof", "Christos", "Clara", "Clara", "Clara", "Clàudia", "Clàudia", "Conor", "Cristina", "Daan", "Dafina", "Daisy", "Daniel", "Daniel", "Daniel", "Daniel", "Daniel", "Daniel", "Daniela", "Daniels", "Daniil", "Daniil", "Daniil", "Danylo", "Daria", "Dariami", "Darius", "Dariya", "Dariya", "Darja", "Darja", "David", "David", "David", "David", "David", "Davide", "Davide", "Davide", "Davit", "Davit", "Dejan", "Demetre", "Denys", "Diana", "Diaz", "Dimitar", "Dimitra", "Dimitra", "Dimitrios", "Dmytro", "Dominykas", "Donika", "Dragan", "Dunja", "Dylan", "Dylan", "Dóra", "Edona", "Eetu", "Egzona", "Elen", "Elena", "Elena", "Elena", "Elena", "Elena", "Elena", "Elena", "Elene", "Eleni", "Eleni", "Eleni", "Elin", "Elina", "Elisa", "Elisa", "Elisabeth", "Elise", "Elise", "Elise", "Eliza", "Elizabete", "Eliška", "Ella", "Ella", "Ella", "Ella", "Ella", "Ellen", "Ellie", "Elsa", "Elísabet", "Ema", "Ema", "Ema", "Emil", "Emil", "Emil", "Emil", "Emil", "Emilia", "Emilia", "Emilia", "Emilia", "Emiliami", "Emilie", "Emilija", "Emilis", "Emils", "Emily", "Emily", "Emily", "Emily", "Emily", "Emily", "Emily", "Emily", "Emilía", "Emina", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emma", "Emīlija", "Eneko", "Enzo", "Ethan", "Ethan", "Ethan", "Ethan", "Ethan", "Ethan", "Eva", "Eva", "Eva", "Eva", "Eva", "Eva", "Evangelos", "Evelinami", "Evelīna", "Fabian", "Farid", "Faris", "Fatma", "Federico", "Felix", "Felix", "Fidan", "Filip", "Filip", "Filip", "Filip", "Filip", "Filip", "Filip", "Finn", "Fleur", "Flori", "Francesco", "Francesco", "Francisco", "Frederik", "Freja", "Gabija", "Gabriel", "Gabriel", "Gabriel", "Gabriel", "Gabriel", "Gabriela", "Gabriele", "Gabrielė", "George", "Georgi", "Georgia", "Georgia", "Georgios", "Gerard", "Gergana", "Gergő", "Giacomo", "Gioele", "Giorgi", "Giorgia", "Giorgia", "Giulia", "Giulia", "Giulia", "Giulia", "Goda", "Grace", "Grace", "Grace", "Gregor", "Guilherme", "Gustav", "Hana", "Hanna", "Hanna", "Hannah", "Hannah", "Hans", "Harry", "Harry", "Harry", "Harry", "Harry", "Harry", "Harry", "Harun", "Hendrik", "Henri", "Holly", "Hugo", "Hugo", "Hugo", "Hugo", "Huseyn", "Ida", "Ida", "Ieva", "Igor", "Iida", "Ilia", "Ilija", "Ilinca", "Ilir", "Ilja", "Ilya", "Imogen", "Ines", "Ingrid", "Inkar", "Innunguaq", "Inuk", "Inzhu", "Inès", "Inês", "Ioana", "Ioannis", "Ion", "Irati", "Isa", "Isla", "Isla", "Isla", "Ivan", "Ivan", "Ivan", "Ivaylo", "Izaro", "Jack", "Jack", "Jack", "Jack", "Jacob", "Jacob", "Jacob", "Jade", "Jake", "Jakob", "Jakob", "Jakub", "James", "James", "James", "James", "James", "James", "Jan", "Jan", "Jan", "Jan", "Jan", "Jan", "Jana", "Janis", "Jessica", "Jessica", "Jessica", "Jessica", "Jessica", "Joana", "Joanna", "Johann", "Johanne", "John", "Jokūbas", "Jon", "Jonas", "Jonas", "Jonas", "Jonas", "Jonas", "Joona", "Josef", "Joseph", "Joshua", "Jules", "Julia", "Julia", "Julia", "Julia", "Julia", "Julia", "Julia", "Julia", "Julie", "Julie", "Julie", "Julie", "Julie", "Julie", "Juliette", "Julija", "June", "Justas", "Jázmin", "Jón", "Júlía", "Kacper", "Kaiden", "Kamilami", "Kamilė", "Karl", "Karolína", "Kaspar", "Katerina", "Kateřina", "Katharina", "Katie", "Katrina", "Katrín", "Katrīna", "Kausar", "Khadija", "Kim", "Kirill", "Kirill", "Kobe", "Konstantina", "Konstantinos", "Kristiyan", "Kristofer", "Kristín", "Kristína", "Kristófer", "Kristýna", "Krystyna", "Kári", "Laia", "Laila", "Laila", "Lamija", "Lana", "Lana", "Lara", "Lara", "Lara", "Lara", "Lara", "Lara", "Lara", "Lara", "Lara", "Lara", "Laura", "Laura", "Laura", "Laura", "Laura", "Laura", "Laura", "Laura", "Laurin", "Lazar", "Lea", "Lea", "Lea", "Lea", "Leandro", "Leandro", "Leandro", "Leevi", "Leire", "Lejla", "Lena", "Lena", "Lena", "Lena", "Lena", "Lena", "Lenny", "Leo", "Leo", "Leon", "Leon", "Leon", "Leonardo", "Leonardo", "Leonardo", "Leonie", "Leonie", "Leonie", "Leonie", "Leonor", "Letizia", "Levente", "Levi", "Levin", "Levin", "Lewis", "Lewis", "Leyla", "Lili", "Lilja", "Lilly", "Lily", "Lily", "Lily", "Lily", "Lily", "Lily", "Linn", "Linnea", "Linnéa", "Lisa", "Lisa", "Lisete", "Lizi", "Ljubica", "Ljupco", "Logan", "Lola", "Lore", "Loreen", "Lorenzo", "Lorenzo", "Lorenzo", "Lotte", "Lotte", "Lotte", "Louis", "Louis", "Louis", "Louis", "Louis", "Louis", "Louise", "Louise", "Louise", "Louise", "Louise", "Luan", "Luana", "Luana", "Luca", "Luca", "Luca", "Luca", "Luca", "Luca", "Luca", "Luca", "Luca", "Luca", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucas", "Lucie", "Lucie", "Lucija", "Lucía", "Lucía", "Lucía", "Luis", "Luis", "Luka", "Luka", "Luka", "Lukas", "Lukáš", "Lukáš", "Luuk", "Léa", "Léa", "Léa", "Léa", "Léa", "Léa", "Léo", "Léo", "Magdalena", "Mahammad", "Maja", "Maja", "Maja", "Maja", "Maksims", "Maksym", "Malea", "Malik", "Maneh", "Manon", "Manon", "Manon", "Manuel", "Marc", "Mari", "Maria", "Maria", "Maria", "Maria", "Maria", "Maria", "Mariam", "Mariam", "Mariami", "Mariana", "Marie", "Marie", "Marie", "Marie", "Marigona", "Marija", "Mario", "Mariya", "Mariya", "Mariya", "Mark", "Mark", "Markel", "Marko", "Marko", "Markus", "Markus", "Markuss", "Marta", "Marten", "Marti", "Martin", "Martin", "Martin", "Martina", "Martina", "Martina", "Martina", "Martina", "María", "María", "María", "Matas", "Mate", "Matei", "Matej", "Mateusz", "Mathias", "Mathias", "Mathilde", "Mathis", "Mathis", "Mathis", "Mathéo", "Matic", "Matilda", "Matilda", "Matteo", "Matteo", "Matteo", "Matthew", "Matthew", "Matthew", "Mattia", "Mattia", "Mattia", "Mattéo", "Matyáš", "Matúš", "Matěj", "Max", "Max", "Maxim", "Maxim", "Maxim", "Maxime", "Maxime", "Maëlys", "Medina", "Melina", "Meribel", "Mia", "Mia", "Mia", "Mia", "Mia", "Mia", "Mia", "Mia", "Mia", "Michael", "Michal", "Michał", "Mikael", "Mikel", "Mikkel", "Milan", "Milan", "Milan", "Milana", "Milena", "Milica", "Milán", "Minik", "Miras", "Mirlinda", "Mirtel", "Mohamed", "Molly", "Molly", "Muhamed", "Nahia", "Najaaraq", "Naomi", "Natalia", "Nathan", "Nathan", "Nathan", "Nathan", "Nathan", "Nathan", "Natália", "Natálie", "Nazar", "Nazrin", "Nejc", "Nela", "Nellie", "Neža", "Nia", "Nico", "Nico", "Nicolò", "Nihad", "Niilo", "Nik", "Nika", "Nikita", "Nikita", "Nikita", "Nikita", "Nikol", "Nikola", "Nikola", "Nikolaos", "Nikolay", "Nina", "Nina", "Nina", "Nina", "Nina", "Nino", "Noah", "Noah", "Noah", "Noah", "Noah", "Noel", "Noemi", "Nolan", "Noor", "Noor", "Nora", "Nora", "Nuka", "Nóra", "Nұrasyl", "Nұrislam", "Oier", "Oliver", "Oliver", "Oliver", "Oliver", "Olivia", "Olivia", "Olivia", "Olivia", "Olivia", "Olivia", "Olivia", "Olivia", "Olivia", "Oliwia", "Omar", "Ondřej", "Onni", "Oscar", "Oskar", "Paninnguaq", "Paraskeui", "Paraskevi", "Patrik", "Pau", "Paul", "Paul", "Paula", "Paula", "Petar", "Petar", "Peter", "Peter", "Petra", "Philip", "Phoebe", "Pipaluk", "Pol", "Polina", "Polina", "Polina", "Rakel", "Ralfs", "Rania", "Raphael", "Raphaël", "Raya", "Rebeca", "Riccardo", "Riccardo", "Riley", "Rinor", "Robbe", "Robin", "Rodigo", "Romain", "Roman", "Romet", "Ronja", "Ruby", "Ruby", "Ryan", "Ryan", "Ryan", "Ryan", "Réka", "Róbert", "Saar", "Saba", "Saga", "Salome", "Samuel", "Samuel", "Samvel", "Sander", "Sanjar", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sara", "Sarah", "Sarah", "Sarah", "Sarah", "Sarah", "Sarah", "Sarah", "Sebastian", "Sebastian", "Sem", "Senne", "Seren", "Sigurður", "Siiri", "Silvano", "Simon", "Simona", "Simone", "Sina", "Smiltė", "Snezana", "Sofia", "Sofia", "Sofia", "Sofia", "Sofia", "Sofia", "Sofia", "Sofia", "Sofiami", "Sofie", "Sofie", "Sofija", "Sofija", "Sofiya", "Sofiya", "Sofía", "Sophia", "Sophia", "Sophia", "Sophia", "Sophia", "Sophie", "Sophie", "Sophie", "Sophie", "Soussana", "Stefan", "Stefan", "Stefania", "Stefán/Stefan", "Steven", "Suzana", "Sára", "Tamar", "Tamari", "Tarik", "Teodora", "Teodora", "Teodora", "Tereza", "Thea", "Thijs", "Thomas", "Thomas", "Thomas", "Thomas", "Thomas", "Thomas", "Théo", "Théo", "Tim", "Tim", "Timur", "Timéo", "Tom", "Tomáš", "Tomáš", "Tunar", "Uiloq", "Uroš", "Uxue", "Valeria", "Valeria", "Valon", "Varvara", "Vasileios", "Vasiliki", "Veeti", "Venla", "Vesna", "Victor", "Victoria", "Viktor", "Viktor", "Viktoria", "Viktoria", "Viktorija", "Viktorija", "Viktoriya", "Viktória", "Vincent", "Viola", "Violeta", "Vuk", "Wiktoria", "William", "William", "William", "William", "William", "William", "Wilma", "Wilma", "Wout", "Yanis", "Yann", "Yara", "Yegor", "Yelizaveta", "Yelizaveta", "Yerasyl", "Zachary", "Zahra", "Zala", "Zeynab", "Zofia", "Zoran", "Zoé", "Zoé", "Zoé", "Zsófia", "Zuzanna"];
var skillNames = ["Confidence", "Driver", "Planner", "Muscles", "Gunman", "Infiltration", "Forgery", "Fence", "Support", "Smuggler"];
var statusNames = ["Clean", "Under Investigation", "Ex-Con", "Wanted", "In Jail", "Dead"];

var agentListLowBound = 0;
var agentListShowCount = 10;

// Job Stuff
var jobRecordValid = [ "name", "desc", "result", "status", "tier", "cost"];
var jobStatusList = [ "offer", "rejected", "failed", "success"];
var currJob;

// Events
// "Name" J# A# A# A# A# A#
// "Complete" J10 A2 A8 A7 A12 A7

// Words List
var nounList = ["Owl", "Hawk", "Sparrow", "Eagle", "Falcon", "Truck", "Car", "Bike", "Plane", "Train", "Sunset", "Sunrise", "Father", "Mother", "Son", "Daughter", "Uncle", "Aunt", "Horse", "Dog", "Cat", "Kitten", "Puppy", "Snake", "Pillow", "Curtain", "Bag", "Suitcase", "Tiger", "Handbag", "Hair", "Toes", "Eyes", "Hand", "Socks", "Pants", "Dress", "Lady", "Lad", "Duke", "Duchess", "Knight", "King", "Queen", "Jack", "Knave", "Squire", "Pickle", "Bananna", "Apple", "Orange", "Pizza", "Bread", "Pasta", "Cheese", "Milk", "Fridge", "Oven", "Ship", "Harbour", "Ferry", "Bus", "Sidewalk", "Path", "Rock", "Dessert", "Desert", "Jungle", "Swamp", "Mountain", "Cave", "Wheel", "Anchor", "Movie", "Game", "Guitar", "Drum", "Flute", "Trumpet", "Fiddle", "Violin", "Face", "Necklace", "Rod", "Desk", "Chair", "Bed", "Dresser", "Mirror", "Roof", "Floor", "House", "Flat", "Apartment", "Plug", "Scissor", "Wand", "Wizard", "Witch", "Warlock"];
var adjectiveList = 
{
    "types" : [
        "Appearance",
        "Color",
        "Condition",
        "Bad",
        "Good",
        "Shape",
        "Size",
        "Sound",
        "Time",
        "Taste",
        "Touch",
        "Quantity"],
    "Appearance": [
        "adorable",
        "beautiful",
        "clean",
        "drab",
        "elegant",
        "fancy",
        "glamorous",
        "handsome",
        "long",
        "magnificent",
        "old-fashioned",
        "plain",
        "quaint",
        "sparkling",
        "ugliest",
        "unsightly",
        "wide-eyed"
    ],
    "Color": [
        "red",
        "orange",
        "yellow",
        "green",
        "blue",
        "purple",
        "gray",
        "black",
        "white"
    ],
    "Condition": [
        "alive",
        "better",
        "careful",
        "clever",
        "dead",
        "easy",
        "famous",
        "gifted",
        "helpful",
        "important",
        "inexpensive",
        "mushy",
        "odd",
        "powerful",
        "rich",
        "shy",
        "tender",
        "uninterested",
        "vast",
        "wrong."
    ],
    "Bad": [
        "angry",
        "bewildered",
        "clumsy",
        "defeated",
        "embarrassed",
        "fierce",
        "grumpy",
        "helpless",
        "itchy",
        "jealous",
        "lazy",
        "mysterious",
        "nervous",
        "obnoxious",
        "panicky",
        "repulsive",
        "scary",
        "thoughtless",
        "uptight",
        "worried"
    ],
    "Good": [
        "agreeable",
        "brave",
        "calm",
        "delightful",
        "eager",
        "faithful",
        "gentle",
        "happy",
        "jolly",
        "kind",
        "lively",
        "nice",
        "obedient",
        "proud",
        "relieved",
        "silly",
        "thankful",
        "victorious",
        "witty",
        "zealous"
    ],
    "Shape": [
        "broad",
        "chubby",
        "crooked",
        "curved",
        "deep",
        "flat",
        "high",
        "hollow",
        "low",
        "narrow",
        "round",
        "shallow",
        "skinny",
        "square",
        "steep",
        "straight",
        "wide."
    ],
    "Size": [
        "big",
        "colossal",
        "fat",
        "gigantic",
        "great",
        "huge",
        "immense",
        "large",
        "little",
        "mammoth",
        "massive",
        "miniature",
        "petite",
        "puny",
        "scrawny",
        "short",
        "small",
        "tall",
        "teeny",
        "teeny-tiny",
        "tiny"
    ],
    "Sound": [
        "cooing",
        "deafening",
        "faint",
        "hissing",
        "loud",
        "melodic",
        "noisy",
        "purring",
        "quiet",
        "raspy",
        "screeching",
        "thundering",
        "voiceless",
        "whispering"
    ],
    "Time": [
        "ancient",
        "brief",
        "early",
        "fast",
        "late",
        "long",
        "modern",
        "old",
        "old-fashioned",
        "quick",
        "rapid",
        "short",
        "slow",
        "swift",
        "young"
    ],
    "Taste": [
        "bitter",
        "delicious",
        "fresh",
        "greasy",
        "juicy",
        "hot",
        "icy",
        "loose",
        "melted",
        "nutritious",
        "prickly",
        "rainy",
        "rotten",
        "salty",
        "sticky",
        "strong",
        "sweet",
        "tart",
        "tasteless",
        "uneven",
        "weak",
        "wet",
        "wooden",
        "yummy"
    ],
    "Touch": [
        "boiling",
        "breeze",
        "broken",
        "bumpy",
        "chilly",
        "cold",
        "cool",
        "creepy",
        "crooked",
        "cuddly",
        "curly",
        "damaged",
        "damp",
        "dirty",
        "dry",
        "dusty",
        "filthy",
        "flaky",
        "fluffy",
        "freezing",
        "hot",
        "warm",
        "wet"
    ],
    "Quantity": [
        "abundant",
        "empty",
        "few",
        "full",
        "heavy",
        "light",
        "many",
        "numerous",
        "sparse",
        "substantial"
    ]
};

//------------------------------------------------------------------------
// Helper Functions
function $(id) { return document.getElementById(id); }
function Log(m) { "use strict"; $("debugLog").innerHTML += m + "\n"; }
function ClearLive() { $("liveOutput").innerHTML = ""; }
function numberWithCommas(x) { return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","); }
function stopEv(e) { e.stopPropagation(); }
function Shuffle(x) 
{ 
    x = x.map(function(x) { var p = {"v": x, "k": m.random()}; return p; }); 
    x = x.sort(function(a,b) { return a.k < b.k; }); 
    x = x.map(function(x) { return x.v; }); 

    x = x.map(function(x) { var p = {"v": x, "k": m.random()}; return p; }); 
    x = x.sort(function(a,b) { return a.k < b.k; }); 
    x = x.map(function(x) { return x.v; }); 

    return x;
}

function Live(label, val, longLabel) 
{
    if(typeof(val) == "number")
    {
        val = val.toFixed(2);
    }

    if($("lo_" + label) === undefined)
    {
        if(longLabel === undefined) { longLabel = label; }
        $("liveOutput").innerHTML += "<dt>" + longLabel + '</dt><dd id="lo_' + label + '">' + val + '</dd>';
    }

    $("lo_" + label).innerHTML = val;
}


function supports_html5_storage() 
{
  try
  {
    return 'localStorage' in window && window.localStorage !== null;
  } 
  catch (e) 
  {
    return false;
  }
}

//---------------------------------------------------------------------
// Core
function createPopup(head,body)
{
    var x = "";

    x += '<div class="blackPopup" id="popup"><h1 id="popHead">';
    x += head;
    x += '</h1><div id="popBody">';
    x += body;
    x += '</div></div>';

    $("gameBox").innerHTML += x;
}

function setPopup(head,body)
{
    var x = $("popup");

    if(x)
    {
        if(head !== undefined)
            $("popHead").innerHTML = head;

        if(body !== undefined)
            $("popBody").innerHTML = body;
    }
    else
    {
        createPopup(head,body);
    }
}

function SaveGame()
{
    localStorage["Player.Name"] = playerName;
    localStorage["Player.Bank"] = playerBank;
    localStorage["OpList"] = opList;
    localStorage["JobList"] = jobList;
}

function InitGame()
{
    if(supports_html5_storage() === false)
    {
        alert("You cannot play Money Manager, Please use a modern browser");
    }

    playerName = localStorage.getItem("Player.Name");
    $("gameBox").style.setProperty("background", 'url(' + imgDotsBG.src + ')');
    $("gameBox").innerHTML = "";

    if(playerName === undefined)
    {
        createPopup('Point of No Return',
            '<p>You want to become a bank for the criminal class?</p> \
            <h4>Enter your name</h4> \
            <form onsubmit="return newPlayer();"><input type="text" id="newName" /><input type="submit" /></form>');
    }
    else
    {
        createPopup("Loading Account", '<p>Welcome back <playerName/></p><span id="scramble">000000000000</span>');
        ReplacePlayerName();

        var c = setInterval(ScrambleData, 1000 / 30);
        setTimeout(function() 
        { 
            clearInterval(c);  
            $("scramble").innerHTML = '<span class="buttonSpan"><a onclick="SelectJobLevel()">Start Game</a></span>';
            LoadGame();
        }, 1000 * 2);
    }
}

function ReplacePlayerName()
{
    var x = document.getElementsByTagName("playerName");

    for (var i = x.length - 1; i >= 0; i--)
        x[i].innerHTML = localStorage["Player.Name"];
}

function ScrambleData() 
{ 
    var x = $("scramble");
    var c = Math.floor(x.innerHTML.length * m.random()); 
    x.innerHTML = x.innerHTML.slice(0,c) + String.fromCharCode('A'.charCodeAt(0) + m.random() * 40) + x.innerHTML.slice(c+1); 
}

function newPlayer()
{

    var newName = $("newName").value;
    if((newName === undefined) || (newName.length < 4))
    {
        alert("Name not valid");
        return false;
    }

    setPopup('Creating Account',
        '<p>I hope you didnt use your real name</p> \
        <span id="scramble">000000000000</span>');

    var c = setInterval(ScrambleData, 1000 / 30);
    setTimeout(function() 
    { 
        clearInterval(c);  
        $("scramble").innerHTML = '<span class="buttonSpan"><a onclick="StartGame()">Start Game</a></span>';
        setPopup('<h3>Account Created</h3>');
        CreateNewGame(newName);
    }, 1000 * 5);

    return false;
}

function CreateNewGame(playerName)
{
    localStorage["Player.Name"] = playerName;

    playerBank = 10000;
    localStorage["Player.Bank"] = playerBank;
    
    opList = [];

    var i;
    for (i=0; i < 8; i++)
        opList = opList.concat(new CreateGuy(0));

    for (i=0; i < 4; i++)
        opList = opList.concat(new CreateGuy(1));

    opList = opList.concat(new CreateGuy(3));
    opList = opList.concat(new CreateGuy(6));
    opList = opList.concat(new CreateGuy(10));
    opList = opList.concat(new CreateGuy(12));

    localStorage["OpList"] = opList;

    jobList = [];
    localStorage["JobList"] = jobList;
}

function CreateNewAgent(skillPoints)
{
    opList = opList.concat(new CreateGuy(skillPoints));
    localStorage["OpList"] = opList;    
}

function archiveJob()
{
    jobList = jobList.concat(currJob);
    localStorage["JobList"] = jobList;
}

function LoadGame()
{
    playerName = localStorage["Player.Name"];
    playerBank = localStorage["Player.Bank"];

    opList = JSON.parse("[" + localStorage["OpList"] + "]");
    opList = opList.map(LoadGuy);
    
    jobList = JSON.parse("[" + localStorage["JobList"] + "]");
    jobList = jobList.map(LoadJob);
}

function StartGame()
{
    // Generate Job
    SelectCreateJob(0);
    DisplayHomeScreen();
}   

function DisplayHomeScreen()
{
    $("gameBox").innerHTML = '';

    $("gameBox").innerHTML += '<div class="jobBox" onclick="DisplayJobDetails()"><h2 id="missionName">Mission Name</h3><div id="currJobData"></div></div>';

    $("gameBox").innerHTML += '<div class="agentList"><h3>Agent List</h3><ul id="agentListData"></ul><span class="buttonSpan" onclick="RecruitAgent();">Recruit New Agent £25,000</span></div>';
    $("gameBox").innerHTML += '<div class="jobList"><h3>Job List</h3><ul id="jobListData"></ul></div>';
    $("gameBox").innerHTML += '<div id="cashBox"><b>Cash</b>:<span id="cashData">000</span></div>';
    $("gameBox").innerHTML += '<div id="policeBox"><span id="policeData">Unknown to the Police</span></div>';

    // Populate Agent List
    var x = $("agentListData");
    for (var i = 0; i < opList.length; i++) 
    {
        var newElem = '<li onclick="DisplayAgent(' + i + 
            ')" class="agentLst_' + opList[i].status + ((i >= agentListShowCount)?(' collapsed'):' ') +
            '" ">' + opList[i].name + '</li>';

        x.innerHTML += newElem;
    }

    DisplayJobTitle();

    // Update Money
    SpendMoney(0);

    $("gameBox").style.setProperty("background", 'url(' + imgHackBG.src + ')');

    SaveGame();
}

function RecruitAgent()
{
    var chance = m.random();

    if(chance < 0.5)
        opList = opList.concat(new CreateGuy(2));
    else if(chance < 0.6)
        opList = opList.concat(new CreateGuy(3));
    else if(chance < 0.7)
        opList = opList.concat(new CreateGuy(4));
    else if(chance < 0.8)
        opList = opList.concat(new CreateGuy(5));
    else if(chance < 0.9)
        opList = opList.concat(new CreateGuy(7));
    else
        opList = opList.concat(new CreateGuy(8));

    DisplayHomeScreen();
}

function GetAgentCost(agent)
{
    return (200 * Math.pow(13, agent.tier));
}

function GetFullMissionCost()
{
    var cost = currJob.cost;

    for (var i = 0; i < currJob.agents.length; i++)
        if(currJob.agents[i] >= 0)
            cost += GetAgentCost(opList[currJob.agents[i]]);

    return Math.floor(cost);
}

function DisplayJobTitle()
{
    var i;
    var jobListBody = "";
    for (i=0; i < jobList.length; i++)
        jobListBody += '<li class="jobLi' + jobList[i].status + '" >' + jobList[i].name + "</li>";

    $("jobListData").innerHTML = jobListBody;

    $("missionName").innerHTML = currJob.name;

    var bodyString = "";
    bodyString = '<p class="desc">' + currJob.desc + "</p>";
    bodyString += '<div class="fadeUpBox"></div>';
    bodyString += '<div id="costInTitle" class="jobCostData"> £' + numberWithCommas(GetFullMissionCost()) + '</div>';
    bodyString += '<div class="riskData">' + currJob.risk + '</div>';

    bodyString += '<div class="tierRating">';
    for (i=0; i < (agentLevels.length - 1); i++)
        if(i < currJob.tier)
            bodyString += '<span class="tierStar yes">&nbsp;</span>';
        else
            bodyString += '<span class="tierStar">&nbsp;</span>';
    bodyString += '</div>';

    bodyString += '<div class="agentJobList" id="agentJobItem">';

    for (i=0; i < currJob.agents.length; i++) 
    {
        bodyString += '<span id="jobAgent' + i + '" onclick="DropAgent(' + i + ');" class="agentSlot';

        if(currJob.agents[i] < 0)
            bodyString += ' empty">&nbsp;</span>';
        else
            bodyString += ' ">#' + currJob.agents[i] + ' - ' + opList[currJob.agents[i]].name + '</span>';
    }

    bodyString += '</div>';
    
    $("currJobData").innerHTML = bodyString;

    setTimeout(stopAgentEv, 2000);
}

function stopAgentEv() { $("agentJobItem").addEventListener("click",stopEv, false); }

function DropAgent(agentSlot)
{
    currJob.agents[agentSlot] = -1;
    
    var x = $("jobAgent" + agentSlot);
    x.innerHTML = '&nbsp;';
    x.className += ' empty';
    $("costInTitle").innerHTML = " £" + numberWithCommas(GetFullMissionCost());
    return false;
}

function HireAgent(agentID)
{
    var i;

    // Check not already hired
    for (i=0; i < currJob.agents.length; i++)
        if(currJob.agents[i] == agentID)
            return i;

    i=0;
    while(i < currJob.agents.length)
    {
        if(currJob.agents[i] < 0)
        {
            currJob.agents[i] = agentID;
            var x = $("jobAgent" + i);
            x.innerHTML = '#' + currJob.agents[i] + ' - ' + opList[agentID].name;
            x.className = "agentSlot";
            $("costInTitle").innerHTML = " £" + numberWithCommas(GetFullMissionCost());
            return i;
        }
        ++i;
    }
    
    alert("No Slots Free");
}

function DisplayJobDetails()
{
    var i;
    var popBody = '';

    popBody += '<span id="costSpanDetail"><b>Cost</b>: £' + numberWithCommas(GetFullMissionCost()) + '</span>';
    popBody += '<span id="riskSpanDetail"><b>Risk</b>: ' + currJob.risk + '</span>';

    popBody += '<p class="jobDetail">' + currJob.desc + '</p>';

    popBody += '<ul class="agentDetailList">';
    for (i=0; i < currJob.agents.length; i++)
        if(currJob.agents[i] < 0)
            popBody += '<li class="empty"> ---- </li>';
        else
            popBody += '<li onclick="DisplayAgent(' + currJob.agents[i] + ')">' + currJob.agents[i] + ' - ' + opList[currJob.agents[i]].name; + '</li>';
    popBody += '</ul>';

    popBody += '<span class="buttonSpan" onclick="ApproveJob();">Approve Job</span>';
    popBody += '<div class="tierRating">';
    for (i=0; i < (agentLevels.length-1); i++)
        if(i < currJob.tier)
            popBody += '<span class="tierStar yes">&nbsp;</span>';
        else
            popBody += '<span class="tierStar">&nbsp;</span>';
    popBody += '</div>';
    popBody += '<span class="buttonSpan" onclick="RejectJob();">Reject Job</span>';

    setPopup(currJob.name, popBody);
}

function ApproveJob()
{
    for (var i = 0; i < currJob.agents.length; i++)
        if(currJob.agents[i] < 0)
        {
            alert("Please Hire 4 Agents");
            return false;
        }

    if(GetFullMissionCost() > playerBank)
    {
        alert("You do not have sufficient funds");
        return false;
    }

    SpendMoney(GetFullMissionCost());
    // Failed currJob.status = 2; 
    // Success currJob.status = 3; 

    // Run Mission Logic
    StartJob();

    if(currJob.logic === undefined)
        currJob.logic = basicRollLogic;
}

function d6() { return Math.floor(m.random() * 6)+1; }

function nd6BestRoll(numRolls)
{
    // Die Roll
    var i;
    var resStr = '';
    var rollData = [];
    var numRes = 0;

    for (i=0; i < numRolls; i++)
        rollData = rollData.concat(d6());

    for (i=rollData.length - 1; i >= 0; i--)
        numRes += Math.pow(10,rollData[i] - 1);

    rollData = rollData.sort();


    // Display Die
    for (i=0; i < rollData.length; i++)
        resStr += '<span class="die res' + rollData[i] + '">&nbsp;</span>';

    this.resStr = resStr;
    this.rollData = rollData;
    this.numRes = numRes;
}

function basicRollLogic()
{
    function BurnAgents()
    {
        $("missionLog").innerHTML += '<span><b class="redText">Mission Failed!</b></span>';

        // Burn Agents
        for (var i = 0; i < currJob.agents.length; i++) 
        {
            var agent = opList[currJob.agents[i]];
            var bad = false;

            while((m.random() < 0.3) && (agent.status < (statusNames.length-1)))
            {
                agent.status += 1;
                bad = true;
                
                if(agent.status == 2)
                    agent.status = 3;
            }

            if(bad)
                $("missionLog").innerHTML += '<span><b class="blueText">' + agent.name + '</b> is <b class="redText">' + statusNames[agent.status]+ '</b>.</span>';
        }

        $("agentsActive").innerHTML = BuildAgentCards();
    }

    if(currJob.rolls.length <= 0)
        return false;

    $("missionLog").innerHTML += '<span>Situation text... roll for <b class="blueText">' + skillNames[currJob.rolls[0]] + '</b></span>';

    var aiRes = new nd6BestRoll(currJob.tier + 1);
    var teamRes = '';
    var bestResult = 0;

    function CheckSixes(x) { return (x == 6); }

    for (var i = 0; i < currJob.agents.length; i++) 
    {
        var agent = opList[currJob.agents[i]];
        var x = agent.skillMap.indexOf(currJob.rolls[0]);
        if(x >= 0)
        {
            var pcRes = new nd6BestRoll(agent.skills[x] + 1);
            teamRes += '<span class="agentNameDie">' + agent.name + '</span>' + pcRes.resStr;

            if(pcRes.rollData.every(CheckSixes))
            {
                agent.skills[x] += 1;
                $("missionLog").innerHTML += '<span><b class="blueText">' + agent.name + '</b> has improved in <b class="blueText">' + skillNames[currJob.rolls[0]] + '</b>.</span>';
                $("agentsActive").innerHTML = BuildAgentCards();
            }

            if(pcRes.numRes > bestResult)
                bestResult = pcRes.numRes;
        }
    }

    if(aiRes.numRes < bestResult)
    {
        aiRes.resStr += '<span class="loseResult">LOSER</span>';
        teamRes += '<span class="winResult">WINNER</span>';

        $("missionLog").innerHTML += '<span>Success string for roll</span>';

        // Do Logic
    }
    else
    {
        aiRes.resStr += '<span class="winResult">WINNER</span>';
        teamRes += '<span class="loseResult">LOSER</span>';

        BurnAgents();
        
        // Failure
        currJob.status = 2;
        currJob.logic = FinishJob;
    }

    $("dieAI").innerHTML = aiRes.resStr;
    $("dieYou").innerHTML = teamRes;

    // Throw away Roll
    currJob.rolls= currJob.rolls.slice(1);
}

function ContinueJob()
{
    if(currJob.logic() === false)
    {
        // Job Over - Success
        playerBank += currJob.reward;
        SpendMoney(0);

        currJob.status = 3;
        currJob.logic = FinishJob;
    }
}

function StopJob()
{
    // Deal with consequence
    currJob.status = 2;

    FinishJob();
}

function StartJob()
{
    $("gameBox").innerHTML = '';

    var sBody = '';

    sBody += '<div class="missionLogOuter"><div id="missionLog"><span>Mission Ready: Continue?</span></div><b class="blinky">_</b></div>';
    
    sBody += '<div class="diceBox">';
    sBody += '<h3>Challenge</h3><div id="dieAI"></div>';
    sBody += '<h3>Your Team</h3><div id="dieYou"></div>';
    sBody += '</div>';

    sBody += '<span id="jobContinueButton" class="buttonSpan" onclick="ContinueJob();">Continue</span>';
    sBody += '<span id="jobQuitButton" class="buttonSpan" onclick="StopJob();">Pull the Plug</span>';

    sBody += '<div id="agentsActive" class="agentsActive">' + BuildAgentCards() + '</div>';

    $("gameBox").style.setProperty("background", 'url(' + imgJobBG.src + ')');

    $("gameBox").innerHTML = sBody;
}

function BuildAgentCards()
{
    var sBody = '';
    for (var i = 0; i < currJob.agents.length; i++) 
    {
        var agent = opList[currJob.agents[i]];
        sBody += '<div class="agentCard slot' + i + '">';
        sBody += '<h3>' + agent.name + '</h3>';
        sBody += GetAgentBody(agent) + '</div>';
    }

    return sBody;
}

function FinishJob()
{
    // Put Away
    SpendMoney(0);
    archiveJob();

    SelectJobLevel();

    SaveGame();
}

function SpendMoney(x)
{
    playerBank -= x;

    if(playerBank < 0)
        alert('OUT OF MONEY');

    localStorage["Player.Bank"] = playerBank;

    if($("cashData") !== undefined)
        $("cashData").innerHTML = "£" + numberWithCommas(playerBank);
}

function RejectJob()
{
    currJob.status = 1;
    archiveJob();

    SelectJobLevel();
}

function DisplayAgent(agentID)
{
    var agent = opList[agentID];

    // Update Scroll
    agentListLowBound = Math.max(0, Math.floor(agentID - (agentListShowCount / 2)));

    var x = $("agentListData").getElementsByTagName("li");
    for (var i = x.length - 1; i >= 0; i--) 
    {
        var newClassName = x[i].className.slice(0,10);
        if(i == agentID)
            newClassName += " active";
        else if((i < agentListLowBound) || ((i - agentListShowCount) > agentListLowBound))
            newClassName += " collapsed";

        x[i].className = newClassName;
    }

    // Show Agent in Popup
    var popBody = GetAgentBody(agent);

    // Hire Button
    if(agent.status == 4)
        popBody += '<span class="buttonSpan" onclick="BreakOutAgent(' + agentID + ')">Jail Break £' + numberWithCommas(GetAgentCost(agent) * 5) + '</span>';
    else if((agent.status == 3) || (agent.status == 2))
        popBody += '<span class="buttonSpan" onclick="NewIDAgent(' + agentID + ')">New ID £' + numberWithCommas(GetAgentCost(agent) * 3) + '</span>';
    else if(agent.status == 1)
        popBody += '<span class="buttonSpan" onclick="BribeDetectiveAgent(' + agentID + ')">Bribe Detective £' + numberWithCommas(GetAgentCost(agent) * 2) + '</span>';

    if(agent.status < 4)
        popBody += '<span class="buttonSpan" onclick="HireAgent(' + agentID + ')">Hire £' + numberWithCommas(GetAgentCost(agent)) + '</span>';


    setPopup(agent.name, popBody);
}


function BreakOutAgent(agent)
{
    agent.status = 3;
    SpendMoney(GetAgentCost(agent) * 5);

    DisplayAgent(agent);
}

function NewIDAgent(agent)
{
    agent.status = 0;
    SpendMoney(GetAgentCost(agent) * 3);

    DisplayAgent(agent);
}

function BribeDetectiveAgent(agent)
{
    agent.status = 0;
    SpendMoney(GetAgentCost(agent) * 2);

    DisplayAgent(agent);
}

function GetAgentBody(agent)
{
    var res = '';

    // Rank and Status
    res += "<h3><em>" + agentLevels[agent.tier] + "</em> - " + statusNames[agent.status] + "</h3>";

    // Skills
    res += "<dl>";
    for (var i = 0; i < 3; i++) 
    {
        res += "<dt>" + skillNames[agent.skillMap[i]] + "</dt><dd>";

        var skillLevel = agent.skills[i];
        while(skillLevel >= 0)
        {
            res += '<div class="tierStar yes">&nbsp;</div>';
            skillLevel--;
        }

        res += "</dd>";
    }
    res += "</dl>";

    return res;
}

function SelectJobLevel()
{
    $("gameBox").innerHTML = "";
    $("gameBox").style.setProperty("background", 'url(' + imgDotsBG.src + ')');

    var bodyText = '';

    for (var i = 0; i < agentLevels.length; i++)
        bodyText += '<span class="buttonSpan" onclick="SelectCreateJob(' + i + ')">' + agentLevels[i] + '</span>';

    setPopup('Select Job Level', bodyText);
}

function SelectCreateJob(tier)
{
    currJob = new CreateJob(tier);
    DisplayHomeScreen();
}

function CreateJob(tier)
{
    this.status = 0;

    if((tier === undefined) || (typeof(tier) != "number") || (tier <= 0) || (tier > 15))
        tier = 0;

    this.tier = Math.max(0,Math.floor(tier));

    this.name = GenerateMissionName();
    this.desc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nisl odio, vehicula condimentum cursus molestie, venenatis quis nisi. Suspendisse non mi sed mauris auctor blandit. Suspendisse potenti. Curabitur hendrerit adipiscing orci vel tincidunt. Sed vitae sem sagittis orci molestie porta a id sem. Curabitur consectetur sagittis eros. Integer vel mauris aliquet lectus sodales dictum at eu libero. Sed elementum metus vel justo dictum auctor. Mauris nec tellus risus, euismod scelerisque massa. Quisque ac mauris non justo pretium pellentesque. Sed pellentesque cursus odio, ac placerat dui pulvinar ut. Mauris eget erat sit amet libero porttitor sagittis vel in nibh.";
    this.result = "--- PENDING APPROVAL ---";

    this.risk = [0.2,0.5];
    this.cost = Math.floor((m.random() * 0.5 + 0.5) * 1000 * Math.pow(13, tier));

    this.agents = [-1,-1,-1,-1];

    this.toString = JobToString;

    // Actual Job Making Part
    this.desc = "<<Generated Text Hint at Rolls needed>> damn 48hr... so instead some random words...";
    this.rolls = [];
    var r = -1;
    while(r < tier)
    {
        var c = Math.floor(m.random() * skillNames.length);
        this.desc += " " + skillNames[c] + " ";
        this.rolls = this.rolls.concat(c);

        r += 0.27;
    }

    this.reward = Math.floor((m.random() * 0.5 + 0.5) * 1000 * Math.pow(13, tier + 1));
}

function LoadJob(x)
{
    x.toString = JobToString;

    // Verify Data
    for (var i = 0; i < jobRecordValid.length; i++)
        if(x[jobRecordValid[i]] === undefined)
            alert("Oops!\n Missing [" + jobRecordValid[i] + "] \n " + x);

    return x;
}

function JobToString()
{ 
    var s = "{";
    s += '"name"   : "' + this.name   + '", ';
    s += '"desc"   : "' + this.desc   + '", ';
    s += '"result" : "' + this.result + '", ';
    s += '"status": '   + this.status + ', ';
    s += '"tier"  : '   + this.tier + ', ';
    s += '"cost"   : '  + this.cost + '}';

    return s;
}

function CreateGuy(skillLevel)
{
    this.name = nameList[Math.floor(m.random() * nameList.length)];
    this.status = 0;

    if((skillLevel === undefined) || (typeof(skillLevel) != "number") || (skillLevel <= 0) || (skillLevel > 15))
        skillLevel = 1;

    skillLevel = Math.max(1,Math.floor(skillLevel));

    // Calculate Rank
    var rank = 1;
    if(skillLevel <= 3)
        rank = skillLevel - 1;
    else if(skillLevel < 8)
        rank = 3;
    else if(skillLevel < 13)
        rank = 4;
    else
        rank = 5;
    this.tier = rank;

    // Pick Random SKills
    var skillShuffle = Shuffle(Shuffle(skillNames.map((function(x){ return skillNames.indexOf(x);}))));
    skills = [1, (skillLevel >= 3)?1:0, (skillLevel >= 5)?1:0];
    skillLevel -= (skillLevel >= 5)?3:(skillLevel >= 3)?2:1;

    // Level Up Skills
    while(skillLevel > 0)
    {
        if((skills[0] <= skills[1]) || (skills[0] <= skills[2]))
            skills[0] += 1;
        else if(skills[1] <= skills[2])
            skills[1] += 1;
        else
            skills[Math.floor(m.random() * 3)] += 1;

        --skillLevel;
    }

    this.skills = skills;
    this.skillMap = skillShuffle.slice(0,3);

    this.toString = GuyToString;
}

function LoadGuy(x)
{
    // Verify Data
    // TODO

    // Setup
    x.toString = GuyToString;
    return x;
}

function GuyToString()
{ 
    var s = "{";
    s += '"name" : "' + this.name + '", ';
    s += '"status" : ' + this.status + ', ';
    s += '"tier" : ' + this.tier + ', ';
    s += '"skills" : [' + this.skills + '], ';
    s += '"skillMap" : [' + this.skillMap + ']}';

    return s;
}

function GenerateMissionName()
{
    var ad = Shuffle(adjectiveList.types);
    var x = Shuffle(adjectiveList[ad[0]])[0] + " ";
    x += Shuffle(adjectiveList[ad[1]])[0] + " ";
    x += Shuffle(nounList)[0];
    x = x.split(' ').map(function(x) { return x[0].toUpperCase() + x.slice(1).toLowerCase(); }).join(' ');

    return x;
}