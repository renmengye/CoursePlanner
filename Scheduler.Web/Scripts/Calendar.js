function GUID() {
    var S4 = function () {
        return Math.floor(
                Math.random() * 0x10000 /* 65536 */
            ).toString(16);
    };
    return S4() + S4();
}

if (!Function.prototype.bind) {
    Function.prototype.bind = function (oThis) {
        if (typeof this !== "function") {
            // closest thing possible to the ECMAScript 5 internal IsCallable function
            throw new TypeError("Function.prototype.bind - what is trying to be bound is not callable");
        }
        var aArgs = Array.prototype.slice.call(arguments, 1),
        fToBind = this,
        fNOP = function () { },
        fBound = function () {
            return fToBind.apply(this instanceof fNOP && oThis
            ? this
            : oThis,
            aArgs.concat(Array.prototype.slice.call(arguments)));
        };
        fNOP.prototype = this.prototype;
        fBound.prototype = new fNOP();
        return fBound;
    };
}

if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
        "use strict";
        if (this === void 0 || this === null) {
            throw new TypeError();
        }
        var t = Object(this);
        var len = t.length >>> 0;
        if (len === 0) {
            return -1;
        }
        var n = 0;
        if (arguments.length > 0) {
            n = Number(arguments[1]);
            if (n !== n) { // shortcut for verifying if it's NaN
                n = 0;
            } else if (n !== 0 && n !== Infinity && n !== -Infinity) {
                n = (n > 0 || -1) * Math.floor(Math.abs(n));
            }
        }
        if (n >= len) {
            return -1;
        }
        var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
        for (; k < len; k++) {
            if (k in t && t[k] === searchElement) {
                return k;
            }
        }
        return -1;
    }
}

function Calendar(placeholder, parent, name) {
    this.items = {};
    this.placeholder = placeholder;
    this.parent = parent;
    this.name = name;
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
// CourseID: number
// Semester: character F S Y
// Day: integer 1-5
// StartTime: integer 0-95
// EndTime: integer 0-95
// Name: string
// Time: string
// Location: string
// Campus: string
// HPosition: calculated horizontal position
// Division: calculated horizontal width divition
// Color: color of the item
Calendar.prototype.addItem = function (item) {
    if (!$(".t" + (item.StartTime - 32)).position()) return;

    item.UID = GUID();

    var times = item.Time.split(" ");
    parsedTime = times[2 * item.Order + 1];
    var locations = "";
    var parsedLocation = "";
    if (item.Location) {
        var locations = item.Location.split(" ");
        parsedLocation = locations[item.Order];
    }

    var uid = item.UID;
    this.items[uid] = item;

    for (var i = item.StartTime; i < item.EndTime; i++) {
        this.itemSlot[item.Day - 1][i].push(item);
    }

    this.refreshDivision(item);

    var content = "<div class='calendarItem course" + item.CourseID + " cal" + item.ID + " " + item.Campus + "' id='" + item.UID + "'><div class='itemInfo'><b>" +
        item.Abbr + "</b><br />" + parsedTime + "<br/>@ " + parsedLocation + "</div></div>";
    //$(this.placeholder).children(".calendarWrapper").append(content);
    $(this.placeholder).find(".calendarContentWrapper").append(content);

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

    $("#" + item.UID).click(function (e) {
        $("#listItem" + this.items[$(e.delegateTarget).attr("id")].CourseID).click();
    }.bind(this));
}

Calendar.prototype.removeItem = function (id) {
    var itemsToBeDeleted = $(this.placeholder).find(".cal" + id);
    for (var i = 0; i < itemsToBeDeleted.length; i++) {
        var uid = $(itemsToBeDeleted[i]).attr("id");
        for (var j = this.items[uid].StartTime; j < this.items[uid].EndTime; j++) {
            var array = this.itemSlot[this.items[uid].Day - 1][j];
            array.splice(array.indexOf(this.items[uid]), 1);
        }
        delete this.items[uid];
    }
    itemsToBeDeleted.remove();
    $("#calItemInfo").hide();
    this.resize();
}

Calendar.prototype.removeCourse = function (courseID) {
    var itemsToBeDeleted = $(this.placeholder).find(".course" + courseID);
    for (var i = 0; i < itemsToBeDeleted.length; i++) {
        var uid = $(itemsToBeDeleted[i]).attr("id");
        for (var j = this.items[uid].StartTime; j < this.items[uid].EndTime; j++) {
            var array = this.itemSlot[this.items[uid].Day - 1][j];
            array.splice(array.indexOf(this.items[uid]), 1);
        }
        delete this.items[uid];
    }
    itemsToBeDeleted.remove();
    $("#calItemInfo").hide();
    this.resize();
}

Calendar.prototype.getBaseCalendar = function () {
    //var content = "<div class='calendarWrapper'><table><tr class='calendarHeader'><td class='name'>" + this.name + "</td><td class='w0 timeGrid'>Mon</td><td class='w1 timeGrid'>Tue</td><td class='w2 timeGrid'>Wed</td><td class='w3 timeGrid'>Thu</td><td class='w4 timeGrid'>Fri</td></tr></table>";
    var content = "<table><tr class='calendarHeader'><td class='name'>" + this.name + "</td><td class='w0 timeGrid'>Mon</td><td class='w1 timeGrid'>Tue</td><td class='w2 timeGrid'>Wed</td><td class='w3 timeGrid'>Thu</td><td class='w4 timeGrid'>Fri</td></tr></table>";
    content += "<div class='calendarContentStatic'><div class='calendarContentWrapper'>";
    content += "<table class='calendarContent'>";
    // 15min for one row, 8am-10pm
    for (var i = 0; i <= 56; i++) {
        content += "<tr class='t" + i + (i % 4 == 0 ? " line" : "") + "'>";
        if (i % 4 == 0) {

            content += "<td class='timeDisplay' rowspan='4'>";
            content += (i / 4 + 8) + ":00";
            content += "</td>";
            for (var j = 0; j < 5; j++) {
                content += "<td class='line'></td>"
            }
        } else {
            for (var j = 0; j < 5; j++) {
                content += "<td></td>"
            }
        }
        content += "</tr>";
    }
    content += "</table></div></div>";
    //content += "</table>";
    return content;
}

Calendar.prototype.refreshDivision = function (item) {
    var maxItems = 0;
    var unavailableSlot = {};
    for (var i = item.StartTime; i < item.EndTime; i++) {
        var slot = this.itemSlot[item.Day - 1][i];
        var itemCount = 0;
        for (var slotItem in slot) {
            if (slot[slotItem].UID) {
                if (slot[slotItem].UID !== item.UID) {
                    unavailableSlot[slot[slotItem].HPosition] = true;
                    slot[slotItem].Division = slot.length;
                    itemCount++;
                }
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

    $(this.placeholder).find(".calendarContentStatic").height($(this.placeholder).height() - 35);
    var timeGrid = $(this.placeholder).find(".timeGrid");

    timeGrid.width(($(this.placeholder).width() - 60) / 5);
    $(this.placeholder).find("td.line").width(($(this.placeholder).width() - 60) / 5);
    $(this.placeholder).find(".timeDisplay").width($(this.placeholder).find(".name").width());
    for (var UID in this.items) {
        var itemObj = $("#" + UID);
        var top = $(this.placeholder).find(".t" + (this.items[UID].StartTime - 32)).position()["top"];
        var bottom = $(this.placeholder).find(".t" + (this.items[UID].EndTime - 32)).position()["top"];
        var height = bottom - top - 3;
        this.refreshDivision(this.items[UID]);
        var width = (timeGrid.width() - 10) / this.items[UID].Division;
        var dayGrid = $(this.placeholder).find(".w" + (this.items[UID].Day - 1));
        var left = dayGrid.position()["left"] - 2 + (width) * this.items[UID].HPosition;
        itemObj.width(width - 5);
        itemObj.height(height);
        itemObj.css("left", left);
        itemObj.css("top", top);

        // Add conflict color
        if (this.items[UID].Division > 1) {
            itemObj.addClass("conflict");
        } else {
            if (itemObj.hasClass("conflict")) {
                itemObj.removeClass("conflict");
            }
        }
    }
}

Calendar.prototype.isConflicted = function (item) {
    var hasItem = this.hasItem(item);
    for (var i = item.StartTime; i < item.EndTime; i++) {
        if (this.itemSlot[item.Day - 1][i].length > 0 && !hasItem) return true;
        if (this.itemSlot[item.Day - 1][i].length > 1) return true;
    }
    return false;
}

Calendar.prototype.hasItem = function (item) {
    return $(this.placeholder).find(".cal" + item.ID).length > 0;
}

Calendar.prototype.tempAdd = function (item) {
    this.tempItem.push(item);
    this.addItem(item);
}

Calendar.prototype.tempRemove = function () {
    this.onTempAnimation = false;
    for (var item in this.tempItem) {
        this.removeItem(this.tempItem[item].ID);
    }
    this.tempItem = [];
}

Calendar.prototype.highlight = function (id) {
    $(this.placeholder).children(".highlight").removeClass("highlight");
    $(this.placeholder).children(".course" + id).addClass("highlight");
}

Calendar.prototype.scrollForItem = function (item) {
    if (this.hasItem(item)) {
        var top = $(".cal" + item.ID).position()["top"];
        if ($(this.placeholder).find(".calendarContentStatic").scrollTop() > top || $(this.placeholder).height() + $(this.placeholder).scrollTop() - 40 < top) {
            $(this.placeholder).find(".calendarContentStatic").animate({ scrollTop: top }, 300);
        }
    }
}

Calendar.prototype.getSelectedSlot = function () {
}

Calendar.prototype.getChildItem = function (id) {
    return $(this.placeholder).children(".cal" + id);
}

function CalendarCollection(placeholder, list) {
    this.placeholder = placeholder;
    this.list = list;
    this.split = true;
    $(placeholder).append("<div id='calendarControlBar' class='widgetTitle2'>" +
        "<div id='downloadIcs' class='calendarControls hasTooltip' data-tooltip='Export iCal'>Export to iCal</div>" +
        "<div id='shareLink' class='calendarControls hasTooltip' data-tooltip='Share Calendar'>Share Timetable</div>" +
        "<div id='splitView' class='calendarControls selected'>Both</div>" +
        "<div id='winterView' class='calendarControls'>Winter</div>" +
        "<div id='fallView' class='calendarControls'>Fall</div>"
        + "</div>");
    $(placeholder).append("<div id='fallCalendar' class='singleCalendar split'></div>");
    $(placeholder).append("<div id='winterCalendar' class='singleCalendar split'></div>");
    this.fallCalendar = new Calendar($("#fallCalendar")[0], this, "Fall");
    this.winterCalendar = new Calendar($("#winterCalendar")[0], this, "Winter");

    $("#fallView").click(function () {
        $(this.fallCalendar.placeholder).hide();
        $(this.winterCalendar.placeholder).hide();
        $(".calendarControls.selected").removeClass("selected");
        $("#fallView").addClass("selected");
        $(this.fallCalendar.placeholder).show("slide", 200, function () {
            this.split = false;
            this.resize();
        }.bind(this));
    }.bind(this));

    $("#winterView").click(function () {
        $(this.fallCalendar.placeholder).hide();
        $(this.winterCalendar.placeholder).hide();
        $(".calendarControls.selected").removeClass("selected");
        $("#winterView").addClass("selected");
        $(this.winterCalendar.placeholder).show("slide", 200, function () {
            this.split = false;
            this.resize();
        }.bind(this));
    }.bind(this));

    $("#splitView").click(function () {
        $(this.fallCalendar.placeholder).hide();
        $(this.winterCalendar.placeholder).hide();
        $(".calendarControls.selected").removeClass("selected");
        $("#splitView").addClass("selected");
        $(this.winterCalendar.placeholder).show("slide", 200);
        $(this.fallCalendar.placeholder).show("slide", 200, function () {
            this.split = true;
            this.resize();
        }.bind(this));
    }.bind(this));

    $("#downloadIcs").click(function () {
        var queryString = this.getAllSectionsString();
        $("#hiddenDownloader").attr("src", "Handlers/ICalHandler.ashx?Courses=" + queryString);
    }.bind(this));

    $("#shareLinkDropDown").css("top", $("#shareLink").offset().top + 35);
    $("#shareLinkDropDown").css("left", $("#shareLink").offset().left);
    $("#shareLinkDropDown").hide();
    $("#shareLink").toggle(function () {
        $("#shareLink").addClass("selected");
        var link = "http://griddy.org?link=" + this.getAllSectionsString();
        $("#shareLinkText").val(link);
        $("#rrLink").attr("href", "http://widget.renren.com/dialog/share?resourceUrl=" + link + "&srcUrl=" + link + "&title=My timetable&description=" + Base64.decode(this.getAllSectionsString()));
        $("#wbLink").attr("href", "http://service.weibo.com/share/share.php?title=My%20Timetable&url=" + link + "&source=bookmark");
        $("#twLink").attr("href", "https://twitter.com/share?url=" + link);
        $("#fbLink").attr("href", "https://www.facebook.com/sharer/sharer.php?u=" + link);
        $("#shareLinkDropDown").show(200);
    }.bind(this),
    function () {
        $("#shareLink").removeClass("selected");
        $("#shareLinkDropDown").hide(200);
    });
    $("#shareLinkText").focus(function () {
        $(this).select();
    });

}

CalendarCollection.prototype.resize = function () {
    $(".singleCalendar").height($(this.placeholder).height() - 40);
    if (!this.split) {
        $(".singleCalendar").width($(this.placeholder).width() - 12);
    } else {
        $(".singleCalendar").width($(this.placeholder).width() / 2 - 11);
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

CalendarCollection.prototype.hasItem = function (item) {
    return this.fallCalendar.hasItem(item) || this.winterCalendar.hasItem(item);
}

CalendarCollection.prototype.highlight = function (id) {
    this.fallCalendar.highlight(id);
    this.winterCalendar.highlight(id);
}

CalendarCollection.prototype.scrollForItem = function (item) {
    this.fallCalendar.scrollForItem(item);
    this.winterCalendar.scrollForItem(item);
}

CalendarCollection.prototype.getAllSectionsString = function () {
    var queryString = "";
    var uniqueMap = {};
    $.each((this.fallCalendar.items), function (key, value) {
        if (!uniqueMap[value.ID]) {
            if (queryString !== "") {
                queryString += ",";
            }
            queryString += value.Abbr;
            uniqueMap[value.ID] = value;
        }
    });
    $.each((this.winterCalendar.items), function (key, value) {
        if (!uniqueMap[value.ID]) {
            if (queryString !== "") {
                queryString += ",";
            }
            queryString += value.Abbr;
            uniqueMap[value.ID] = value;
        }
    });
    return Base64.encode(queryString);
}

CalendarCollection.prototype.removeCourse = function (courseID) {
    this.fallCalendar.removeCourse(courseID);
    this.winterCalendar.removeCourse(courseID);
}