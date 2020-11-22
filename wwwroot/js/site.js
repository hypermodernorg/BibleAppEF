
function UpdateRole(RoleId) {

    $.ajax({
        type: "POST",
        url: '/Identity/Admin/Roles/Edit?handler=Update',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("BIBLEAPP-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { RoleId: RoleId }
    });
}


function UpdateRole2(RoleId) {

    var NEWNAME = document.getElementById(newName).value;
    //roletodelete.remove();

    $.ajax({
        type: "POST",
        url: '/Identity/Admin/Roles/Edit?handler=Update2',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("BIBLEAPP-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { RoleId: RoleId, NewName: NEWNAME }
    });
}

function DeleteRole(RoleId) {

    // First item created for some reason first element created cannot be removed from page due to
    // invalidRoleName error even though it is deleted in the database. Therefore, remove it manually 
    // here.
    var roletodelete = document.getElementById(RoleId);
    roletodelete.remove();

    $.ajax({
        type: "POST",
        url: '/Identity/Admin/Roles?handler=Delete',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("BIBLEAPP-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { RoleId: RoleId }
    });
}

function onchangeVersion() {
    var version = document.getElementById("versionSelect").value;
    //document.getElementById("versionHeader").innerText = version;
    $.ajax({
        type: "POST",
        url: '/Registers/UpdateBooks',
        data: { Version: version },
        dataType: "json",
        success: function (jsonObject) {

            booksSelect = document.getElementById("bookSelect");
            document.getElementById("versionHeader").innerText = jsonObject.Version[0].Name;
            $("#bookSelect").empty();
            $("#chapterSelect").empty();
            $("#verseSelect").empty();
            var optStart = document.createElement('option');
            booksSelect.appendChild(optStart);
            for (var i in jsonObject.BookList) {

                var opt = document.createElement('option');

                // create text node to add to option element (opt)
                //opt.appendChild(document.createTextNode(bookKeyValues(jsonObject[i].Book)));
                opt.appendChild(document.createTextNode(bookKeyValues(jsonObject.BookList[i].Book)));

                // set value property of opt
                opt.value = jsonObject.BookList[i].Book;

                // add opt to end of select box (sel)
                booksSelect.appendChild(opt); 
            }

        },
        error: function (e) {
            alert('fail');
        }
    });

}

function onchangeBook() {
    var version = document.getElementById("versionSelect").value;
    var book = document.getElementById("bookSelect").value;
    $.ajax({
        type: "POST",
        url: '/Registers/UpdateChapters',
        data: { Version: version, Book: book },
        dataType: "json",
        success: function (jsonChapters) {

            chapterSelect = document.getElementById("chapterSelect");
            $("#chapterSelect").empty();
            $("#verseSelect").empty();
            var optStart = document.createElement('option');
            chapterSelect.appendChild(optStart);
            for (var i in jsonChapters) {

                var opt = document.createElement('option');

                // create text node to add to option element (opt)
                opt.appendChild(document.createTextNode(jsonChapters[i].Chapter));

                // set value property of opt
                opt.value = jsonChapters[i].Chapter;

                // add opt to end of select box (sel)
                chapterSelect.appendChild(opt);
            }
        },
        error: function (e) {
            alert('fail');
        }
    });

}

function onchangeChapter() {
    var version = document.getElementById("versionSelect").value;
    var book = document.getElementById("bookSelect").value;
    var chapter = document.getElementById("chapterSelect").value;
    $.ajax({
        type: "POST",
        url: '/Registers/UpdateVerses',
        data: { Version: version, Book: book, Chapter: chapter },
        dataType: "json",
        success: function (jsonVerses) {

            verseSelect = document.getElementById("verseSelect");
            $("#verseSelect").empty();

            var optStart = document.createElement('option');
            verseSelect.appendChild(optStart);

            for (var i in jsonVerses) {

                var opt = document.createElement('option');

                // create text node to add to option element (opt)
                opt.appendChild(document.createTextNode(jsonVerses[i].Verse));

                // set value property of opt
                opt.value = jsonVerses[i].Verse;

                // add opt to end of select box (sel)
                verseSelect.appendChild(opt);
            }
        },
        error: function (e) {
            alert('fail');
        }
    });
}

function onChange(event) {
    var file = event.target.files[0];
    var reader = new FileReader();
    reader.onload = function (e) {
        // The file's text will be printed here
        var lines = this.result.split('\n');
        var list = [];
        for (var line = 0; line < lines.length; line++) {
            list.push(lines[line]);
        }

        var source = lines[0].replace("#", "").trim();
        var name = lines[1].replace("#name", "").trim();
        var filetype = lines[2].replace("#filetype", "").trim();
        var copyright = lines[3].replace("#copyright", "").trim();
        var abbreviation = lines[4].replace("#abbreviation", "").trim();
        var language = lines[5].replace("#language", "").trim();
        var note = lines[6].replace("#note", "").trim();

        document.getElementById("Source").value = source;
        document.getElementById("Name").value = name;
        document.getElementById("FileType").value = filetype;
        document.getElementById("Copyright").value = copyright;
        document.getElementById("Abbreviation").value = abbreviation;
        document.getElementById("Language").value = language;
        document.getElementById("Note").value = note;
    };

    reader.readAsText(file);
}

function bookKeyValues(book) {

    // add the b because javascript doesnt like to start model properties with numbers.
    book = "b" + book;

    
    var bookkeyvalues = {
        b01O: "Genesis",
        b02O: "Exodus",
        b03O: "Leviticus",
        b04O: "Numbers",
        b05O: "Deuteronomy",
        b06O: "Joshua",
        b07O: "Judges",
        b08O: "Ruth",
        b09O: "1 Samuel",
        b10O: "2 Samuel",
        b11O: "1 Kings",
        b12O: "2 Kings",
        b13O: "1 Chronicles",
        b14O: "2 Chronicles",
        b15O: "Ezra",
        b16O: "Nehemiah",
        b17O: "Esther",
        b18O: "Job",
        b19O: "Psalms",
        b20O: "Proverbs",
        b21O: "Ecclesiastes",
        b22O: "Song of Solomon",
        b23O: "Isaiah",
        b24O: "Jeremiah",
        b25O: "Lamentations",
        b26O: "Ezekiel",
        b27O: "Daniel",
        b28O: "Hosea",
        b29O: "Joel",
        b30O: "Amos",
        b31O: "Obadiah",
        b32O: "Jonah",
        b33O: "Micah",
        b34O: "Nahum",
        b35O: "Habakkuk",
        b36O: "Zephaniah",
        b37O: "Haggai",
        b38O: "Zechariah",
        b39O: "Malachi",
        b40N: "Matthew",
        b41N: "Mark",
        b42N: "Luke",
        b43N: "John",
        b44N: "Acts of the Apostles",
        b45N: "Romans",
        b46N: "1 Corinthians",
        b47N: "2 Corinthians",
        b48N: "Galatians",
        b49N: "Ephesians",
        b50N: "Philippians",
        b51N: "Colossians",
        b52N: "1 Thessalonians",
        b53N: "2 Thessalonians",
        b54N: "1 Timothy",
        b55N: "2 Timothy",
        b56N: "Titus",
        b57N: "Philemon",
        b58N: "Hebrews",
        b59N: "James",
        b60N: "1 Peter",
        b61N: "2 Peter",
        b62N: "1 John",
        b63N: "2 John",
        b64N: "3 John",
        b65N: "Jude",
        b66N: "Revelation",
        b67A: "Tobit",
        b68A: "Judith",
        b69A: "Esther (Greek)",
        b70A: "Wisdom of Solomon",
        b71A: "Ecclesiasticus (Sira)",
        b72A: "Baruch",
        b73A: "Epistle of Jeremiah",
        b74A: "Prayer of Azariah",
        b75A: "Susanna",
        b76A: "Bel and the Dragon",
        b77A: "1 Maccabees",
        b78A: "2 Maccabees",
        b79A: "3 Maccabees",
        b80A: "4 Maccabees",
        b81A: "1 Esdras",
        b82A: "2 Esdras",
        b83A: "Prayer of Manasseh",
        b84A: "Psalm 151",
        b85A: "Psalm of Solomon",
        b86A: "Odes"
    };
    return bookkeyvalues[book];
}

function SubmitSearch() {

    var version = document.getElementById("versionSelect").value;
    var book = document.getElementById("bookSelect").value;
    var chapter = document.getElementById("chapterSelect").value;
    var verse = document.getElementById("verseSelect").value;
    var wordstosearch = document.getElementById("SearchWords").value;
    var nottosearch = document.getElementById("NotWords").value;
    var searchmode = document.querySelector('input[name="wordRadio"]:checked').value
    var notmode = document.querySelector('input[name="notRadio"]:checked').value
    //document.getElementById("versionHeader").innerText = version;


    $.ajax({
        type: "POST",
        url: '/Registers/GetBible',
        data: { Version: version, Book: book, Chapter: chapter, Verse: verse, WordsToSearch: wordstosearch, SearchMode: searchmode, NotMode: notmode, NotToSearch: nottosearch},
        dataType: "html",
        success: function (jsonObject) {
            //alert('great success');
            document.getElementById("bibleText").innerHTML = jsonObject;
        },
        error: function (e) {
            alert('fail');
        }
    });
}

function LinkSearch(searchwords) {

    var version = document.getElementById("versionSelect").value;
 
    //document.getElementById("versionHeader").innerText = version;


    $.ajax({
        type: "POST",
        url: '/Registers/GetBible',
        data: { Version: version, WordsToSearch: searchwords },
        dataType: "html",
        success: function (jsonObject) {
            //alert('great success');
            document.getElementById("bibleText").innerHTML = jsonObject;
        },
        error: function (e) {
            alert('fail');
        }
    });
}