// Callback from a <input type="file" onchange="onChange(event)">
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
