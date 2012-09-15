var searchStringTemp = "";
var onAnimation = false;
var calendar;
var courseInfo;

$(document).ready(function () {
    $(window).resize(layout);
    $("#searchBox").keyup(function () {
        searchStringTemp = $("#searchBox").val();
        setTimeout(function () {
            if ($("#searchBox").val() === searchStringTemp) {
                runSearch(searchStringTemp);
            }
        }, 200);
    });

    $(document).click(function (e) {
        $("#courseInfo").hide();
    });

    calendarViews.innerHTML = "<div id='tempCalendar' class='singleCalendar'></div>"
    calendar = new Calendar(document.getElementById("tempCalendar"));

    layout();
});

function layout() {
    $("#pageWrapper").height(window.innerHeight - 20);
    $("#resultView").height($("#sideBar").height() - 30);
    $("#searchBox").width($("#sideBar").width() - 11);
    $("#calendarViews").width(window.innerWidth - $("#sideBar").width() - 10);
    $("#calendarViews").height($("#sideBar").height());

    $(".singleCalendar").width($("#calendarViews").width() - 5);
    $(".singleCalendar").height($("#calendarViews").height() - 10);

    if (calendar) {
        calendar.resize();
    }
}

function runSearch(query) {
    if (query.length > 1) {
        var fac = getUrlParameters()["faculty"];
        if (fac) {
            query += " fac:" + fac;
        }
        $.getJSON("api/search?q=" + escape(query), function (result) {
            new ResultView($("#resultView")[0], result);
        });
    }
}

function searchCourseInfo(id, target) {
    $.getJSON("api/search?id=" + parseInt(id.substring(1)), function (course) { courseInfo = new CourseInfo($("#courseInfo"), course, target); });
}

// ResultView object
function ResultView(placeholder, result) {
    this.placeholder = placeholder;
    this.result = result;
    this.formatElements(placeholder, result);
}

ResultView.prototype.formatElements = function (placeholder, result) {
    $(placeholder).html("");

    function addCollection(collection, name, title, collapsed) {
        if (collection) {
            $(placeholder).append("<div class='matchCollection" + (collapsed ? " collapsed" : "") + "' id='" + name + "'></div>");
            new MatchCollection($("#" + name), title, collection);
        }
    }

    addCollection(result.CodeNameMatches, "codeNameMatches", "Search Result", false);
    addCollection(result.DescriptionMatches, "desMatches", "Relevant Description", true);
    addCollection(result.DepartmentMatches, "depMatches", "Relevant Department", true);
    addCollection(result.PrerequisiteMatches, "preqMatches", "Relevant Prerequisite", true);
    addCollection(result.RawMatches, "rawMatches", "Other Relevant Courses", true);

    $(".matchCollection.collapsed").height(20);
}

// ResultView > MatchCollection object
function MatchCollection(placeholder, name, collection) {
    this.placeholder = placeholder;
    this.collection = this.rearrangeCollection(collection);

    var content = "<div class='matchCollectionTitle'>" + name + "</div><ul>" + collection + "</ul>";

    $(this.placeholder).html(content);
    $(this.placeholder).children(".matchCollectionTitle").click(this.onTitleClick);
    $(this.placeholder).children("ul").children(".courseResult").click(function (e) {
        searchCourseInfo($(e.target).attr("id"), $(e.target));
    });
}

MatchCollection.prototype.rearrangeCollection = function (collection) {
    var result = {};
    for (var i = 0; i < collection.length; i++) {
        var id = collection[i].ID;
        result.id = collection[i];
    }
    return result;
}

MatchCollection.prototype.onTitleClick = function (e) {
    var target = $(e.target);
    var placeholder = target.parent();
    var parent = placeholder.parent();

    if (placeholder.hasClass("collapsed")) {
        var onAnimation = true;
        placeholder.animate({ height: target.next().children().length * 24 + 20 }, 200, function () {

            var scrollTop = parent.scrollTop();
            var top = target.position()["top"];

            if (placeholder.height() > parent.height()) {
                parent.scrollTop(scrollTop + top - 40);
            } else {
                parent.scrollTop(scrollTop + top - parent.height() + placeholder.height());
            }
        });
        placeholder.removeClass("collapsed");
    } else {
        placeholder.animate({ height: 20 }, 200);
        placeholder.addClass("collapsed");
    }
}

// CourseInfo object
function CourseInfo(placeholder, course, alignment) {
    this.course = course;
    this.placeholder = placeholder;

    $(this.placeholder).html(this.getHtml(this.course));
    var top = $(alignment).offset()["top"];
    if (top + $(this.placeholder).height() > window.innerHeight - 10) {
        $(this.placeholder).css("top", top - $(this.placeholder).height() + 20);
    } else {
        $(this.placeholder).css("top", top);
    }
    $(this.placeholder).show();
}

CourseInfo.prototype.getHtml = function (course) {
    var content;
    content = "<div id='courseInfoTitle'><b>" + course.Abbr + ": " + course.Name + "</b></div>";
    if (course.Description) {
        content += "<div class='courseInfoDescription'>" + course.Description + "</div>";
    }

    // Add lecture sections
    content += "<div id='courseInfoLectureSections'><b>Sections:</b>";
    for (var i = 0; i < course.Sections.length; i++) {
        if (course.Sections[i].IsLecture) {
            content += "<div class='section' onclick='courseInfo.addSection(" + i + ")'>" + course.Sections[i].Name + "</div>";
        }
    }
    content += "</div>";

    // Add non-lecture sections
    content += "<div id='courseInfoOtherSections'>";
    for (var i = 0; i < course.Sections.length; i++) {
        if (!course.Sections[i].IsLecture) {
            content += "<div class='section' onclick='courseInfo.addSection(" + i + ")'>" + course.Sections[i].Name + "</div>";
        }
    }
    content += "</div>";

    if (course.Prerequisites) {
        content += "<div class='courseInfoDescription'><b>Prerequisites: </b>" + course.Prerequisites + "</div>";
    }
    if (course.Corequisites) {
        content += "<div class='courseInfoDescription'><b>Corequisites: </b>" + course.Corequisites + "</div>";
    }
    if (course.Exclusions) {
        content += "<div class='courseInfoDescription'><b>Exclusions: </b>" + course.Exclusions + "</div>";
    }
    return content;
}

CourseInfo.prototype.addSection = function (i) {
    var meetTimes = this.course.Sections[i].ParsedTime.MeetTimes;
    for (var j = 0; j < meetTimes.length; j++) {
        calendar.addItem({
            Name: this.course.Abbr + ": " + this.course.Name,
            ID: this.course.ID + this.course.Sections[i].Name,
            Day: meetTimes[j].Day,
            StartTime: meetTimes[j].Start,
            EndTime: meetTimes[j].End
        });
    }
}

/* Method to Parse Url Parameters */
function getUrlParameters() {
    var urlParams = {};
    var e,
                a = /\+/g,  // Regex for replacing addition symbol with a space
                r = /([^&=]+)=?([^&]*)/g,
                d = function (s) { return decodeURIComponent(s.replace(a, " ")); },
                q = window.location.search.substring(1);

    while (e = r.exec(q))
        urlParams[d(e[1])] = d(e[2]);

    return urlParams;
}