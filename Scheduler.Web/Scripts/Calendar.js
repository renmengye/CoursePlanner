function GUID() {
    var S4 = function () {
        return Math.floor(
                Math.random() * 0x10000 /* 65536 */
            ).toString(16);
    };

    return (
            S4() + S4() + "-" +
            S4() + "-" +
            S4() + "-" +
            S4() + "-" +
            S4() + S4() + S4()
        );
}

function Calendar(placeholder, parent) {
    this.items = {};
    this.placeholder = placeholder;
    this.parent = parent;
    this.placeholder.innerHTML = this.getBaseCalendar();
    this.itemSlot = new Array(5);
    this.tempItem = [];

    for (var i = 0; i < 5; i++) {
        this.itemSlot[i] = new Array(96);
        for (var j = 0; j < 96; j++) {
            this.itemSlot[i][j] = new Array();
        }
    }

    $(".timeGrid").width(($(this.placeholder).width() - 40) / 5);
}

// Item has the following properties:
// ID: string
// Semester: character F S Y
// Day: integer 1-5
// StartTime: integer 0-95
// EndTime: integer 0-95
// Name: string
// Time: string
// Location: string
// HPosition: calculated horizontal position
// Division: calculated horizontal width divition
Calendar.prototype.addItem = function (item) {
    var top = $(".t" + (item.StartTime - 32)).position()["top"];
    var bottom = $(".t" + (item.EndTime - 32)).position()["top"];
    var height = bottom - top - 5;
    var width = $(".timeGrid").width() - 15;
    var left = $(".w" + (item.Day - 1)).position()["left"] + 5;
    item.UID = GUID();

    var times = item.Time.split(" ");
    parsedTime = times[2 * item.Order + 1];
    var locations = item.Location.split(" ");
    parsedLocation = locations[item.Order];

    var uid = item.UID;
    this.items[uid] = item;

    for (var i = item.StartTime; i < item.EndTime; i++) {
        this.itemSlot[item.Day - 1][i].push(item);
    }

    this.refreshDivision(item);
    width = width / (item.Division);
    left += width * item.HPosition;

    var content = "<div class='calendarItem cal" + item.ID + "' id='" + item.UID + "' style='height:" + height + "px;width:" + width +
        "px;top:" + top + "px;left:" + left + "px;'><div class='itemInfo'><b>" +
        item.Name + "</b><br />" + parsedTime + "<br/>" + parsedLocation +
        "</div><div class='removeItem'>Delete</div></div>";
    $(this.placeholder).children("table").append(content);

    $("#" + item.UID).children(".removeItem").click(function () {
        this.parent.removeItem(item.ID);
    }.bind(this));

    this.resize();

    $("#" + item.UID).hover(function () {
        $(".cal" + item.ID).each(function () {
            $(this).addClass("hover");
        });
    }, function () {
        $(".cal" + item.ID).each(function () {
            $(this).removeClass("hover");
        });
    });
}

Calendar.prototype.removeItem = function (id) {
    var itemsToBeDeleted = $(this.placeholder).children().children(".cal" + id);
    for (var i = 0; i < itemsToBeDeleted.length; i++) {
        var uid = $(itemsToBeDeleted[i]).attr("id");
        for (var j = this.items[uid].StartTime; j < this.items[uid].EndTime; j++) {
            var array = this.itemSlot[this.items[uid].Day - 1][j];
            array.splice(array.indexOf(this.items[uid]), 1);
        }
        delete this.items[uid];
    }
    itemsToBeDeleted.remove();
    this.resize();
}

Calendar.prototype.getBaseCalendar = function () {
    var content = "<table><tr class='calendarHeader'><td class='timeDisplay'></td><td class='w0 timeGrid'>Mon</td><td class='w1 timeGrid'>Tue</td><td class='w2 timeGrid'>Wed</td><td class='w3 timeGrid'>Thu</td><td class='w4 timeGrid'>Fri</td></tr>";

    // 15min for one row, 8am-10pm
    for (var i = 0; i <= 56; i++) {
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

Calendar.prototype.refreshDivision = function (item) {
    var maxItems = 0;
    var unavailableSlot = {};
    for (var i = item.StartTime; i < item.EndTime; i++) {
        var slot = this.itemSlot[item.Day - 1][i];
        var itemCount = 0;
        for (var slotItem in slot) {
            if (slot[slotItem].UID != item.UID) {
                unavailableSlot[slot[slotItem].HPosition] = true;
                slot[slotItem].Division = slot.length;
                itemCount++;
            }
        }
        if (itemCount > maxItems) maxItems = itemCount;
    }
    item.Division = maxItems + 1;
    var slot = 0;
    for (var i = 0; i <= maxItems; i++) {
        if (!unavailableSlot[i]) slot = i;
    }
    item.HPosition = slot;
}

Calendar.prototype.resize = function () {
    $(".timeGrid").width(($(this.placeholder).width() - 40) / 5);
    for (var UID in this.items) {
        var itemObj = $("#" + UID)
        this.refreshDivision(this.items[UID]);
        var width = ($(".timeGrid").width() - 10) / this.items[UID].Division;
        var left = $(".w" + (this.items[UID].Day - 1)).position()["left"] + 5 + (width) * this.items[UID].HPosition;
        itemObj.width(width - 5);
        itemObj.css("left", left);
    }
}

Calendar.prototype.isConflicted = function (item) {
    for (var i = item.StartTime; i < item.EndTime; i++) {
        if (this.itemSlot[item.Day - 1][i].length > 0) return true;
    }
    return false;
}

Calendar.prototype.tempAdd = function (item) {
    this.tempItem.push(item);
    this.addItem(item);
}

Calendar.prototype.tempRemove = function () {
    for (var item in this.tempItem) {
        this.removeItem(this.tempItem[item].ID);
    }
    this.tempItem = [];
}

function CalendarCollection(placeholder) {
    this.placeholder = placeholder;
    this.splitted = true;
    $(placeholder).append("<div id='fallCalendar' class='singleCalendar splitted'></div>");
    $(placeholder).append("<div id='winterCalendar' class='singleCalendar splitted'></div>");
    this.fallCalendar = new Calendar($("#fallCalendar")[0], this);
    this.winterCalendar = new Calendar($("#winterCalendar")[0], this);
}

CalendarCollection.prototype.resize = function () {
    $(".singleCalendar").height($(this.placeholder).height() - 10);
    if (!this.splitted) {
        $(".singleCalendar").width($(this.placeholder).width() - 5);
    } else {
        $(".singleCalendar").width($(this.placeholder).width() / 2 - 5);
    }
    this.fallCalendar.resize();
    this.winterCalendar.resize();
}

CalendarCollection.prototype.addItem = function (item) {
    if (item.Semester == "F" || item.Semester == "Y") {
        this.fallCalendar.addItem(item);
    }
    if (item.Semester == "S" || item.Semester == "Y") {
        this.winterCalendar.addItem(item);
    }
}

CalendarCollection.prototype.removeItem = function (id) {
    this.fallCalendar.removeItem(id);
    this.winterCalendar.removeItem(id);
}

CalendarCollection.prototype.tempAdd = function (item) {
    if (item.Semester == "F" || item.Semester == "Y") {
        this.fallCalendar.tempAdd(item);
    }
    if (item.Semester == "S" || item.Semester == "Y") {
        this.winterCalendar.tempAdd(item);
    }
}

CalendarCollection.prototype.tempRemove = function () {
    this.fallCalendar.tempRemove();
    this.winterCalendar.tempRemove();
}

CalendarCollection.prototype.isConflicted = function (item) {
    if (item.Semester === "F" || item.Semester === "Y") {
        if (this.fallCalendar.isConflicted(item)) {
            return true;
        }
    }
    if (item.Semester === "S" || item.Semester === "Y") {
        if (this.winterCalendar.isConflicted(item)) {
            return true;
        }
    }
    return false;
    //return this.fallCalendar.isConflicted(item) || this.winterCalendar.isConflicted(item);
}