function Calendar(placeholder) {
    this.items = [];
    this.placeholder = placeholder;
    this.placeholder.innerHTML = this.getBaseCalendar();
    $(".timeGrid").width(($(this.placeholder).width() - 40) / 5);
}

// Item has the following properties:
// ID: integer
// Day: integer 1-5
// StartTime: integer 0-95
// EndTime: integer 0-95
// Name: string
// Description: string
Calendar.prototype.addItem = function (item) {
    var top = $(".t" + (item.StartTime - 32)).position()["top"];
    var bottom = $(".t" + (item.EndTime - 32)).position()["top"];
    var height = bottom - top - 4;
    var width = $(".timeGrid").width() - 4;
    var left = $(".w" + (item.Day - 1)).position()["left"];
    var content = "<div class='calendarItem cal" + item.ID + "' style='height:" + height + "px;width:" + width +
        "px;top:" + top + "px;left:" + left + "px;'>" + item.Name + "</div>";
    $(this.placeholder).children("table").append(content);
    this.items.push(item);
}

Calendar.prototype.getBaseCalendar = function () {
    var content = "<table><tr class='calendarHeader'><td class='timeDisplay'></td><td class='w0 timeGrid'>Mon</td><td class='w1 timeGrid'>Tue</td><td class='w2 timeGrid'>Wed</td><td class='w3 timeGrid'>Thu</td><td class='w4 timeGrid'>Fri</td></tr>";

    // 15min for one row, 8am-10pm
    for (var i = 0; i < 56; i++) {
        content += "<tr class='t" + i + (i % 4 == 0 ? " line" : "") + "'>";
        if (i % 4 == 0) {

            content += "<td class='timeDisplay' rowspan='4'>";
            content += (i / 4 + 8) + ":00";
            content += "</td>";
        }
        for (var j = 0; j < 5; j++) {
            content += "<td></td>"
        }
        content += "</tr>";
    }
    content += "</table>";
    return content;
}

Calendar.prototype.resize = function () {
    $(".timeGrid").width(($(this.placeholder).width() - 40) / 5);
    for (var i = 0; i < this.items.length; i++) {
        var allItems=$(".cal" + this.items[i].ID);
        for(var j=0;j<allItems.length;j++){
            var width = $(".timeGrid").width() - 4;
            var left = $(".w" + (this.items[i].Day - 1)).position()["left"];
            $(allItems[j]).width(width);
            $(allItems[j]).css("left", left);
        }
    }
}