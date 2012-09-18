var searchStringTemp = "";
var progSearchStringTemp = "";
var onAnimation = false;
var calendar;
var courseInfo;

$(document).ready(function () {
    $(window).resize(layout);

    calendar = new CalendarCollection(document.getElementById("calendarViews"));
    programInfo = new ProgramInfo(document.getElementById("programInfo"));

    $("#searchBox").keyup(function () {
        searchStringTemp = $("#searchBox").val();
        setTimeout(function () {
            if ($("#searchBox").val() === searchStringTemp) {
                searchCourse(searchStringTemp);
            }
        }, 200);
    });

    $("#programSearchBox").keyup(function () {
        progSearchStringTemp = $("#programSearchBox").val();
        setTimeout(function () {
            if ($("#programSearchBox").val() === progSearchStringTemp) {
                searchProgram(progSearchStringTemp);
            }
        }, 200);
    });

    $(document).click(function (e) {
        if ($(e.target).attr("id") !== "courseInfo" && $(e.target).parent().attr("id") !== "courseInfo" && $(e.target).parent().parent().attr("id") !== "courseInfo") {
            $("#courseInfo").hide();
        }
    });

    $("#programInfoButton").click(function (e) {
        if ($(this).hasClass("expand")) {
            $("#programInfo").hide();
            $(this).removeClass("expand");
        } else {
            $("#programInfo").css("left", window.innerWidth - $("#programInfo").width() - 5);
            $("#programInfo").show();
            $(this).addClass("expand");
        }
    });
    layout();
});

function layout() {
    $("#pageWrapper").height(window.innerHeight - 50);
    $("#resultView").height($("#sideBar").height() - 30);
    $("#searchBox").width($("#sideBar").width() - 11);
    $("#calendarViews").width(window.innerWidth - $("#sideBar").width() - 10);
    $("#calendarViews").height($("#sideBar").height());

    if (calendar) {
        calendar.resize();
    }
}

function searchCourse(query) {
    if (query.length > 1) {
        var fac = getUrlParameters()["faculty"];
        if (fac) {
            query += " fac:" + fac;
        }
        $.getJSON("api/course?q=" + escape(query), function (result) {
            new ResultView($("#resultView")[0], result);
        });
    }
}

function searchProgram(query) {
    if (query.length > 1) {
        $.getJSON("api/program?q=" + escape(query), programInfo.loadSearches);
    }
}

function searchCourseInfo(id, target) {
    $.getJSON("api/course?id=" + parseInt(id.substring(1)), function (course) { courseInfo = new CourseInfo($("#courseInfo"), course, target); });
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
    addCollection(result.PostrequisiteMatches, "postMatches", "Relevant Prerequisite", false);
    addCollection(result.DescriptionMatches, "desMatches", "Relevant Description", true);
    addCollection(result.DepartmentMatches, "depMatches", "Relevant Department", true);
    addCollection(result.PrerequisiteMatches, "preqMatches", "Relevant Postrequisite", true);
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

    $(".section").hover(function () {
        courseInfo.addSection($(this).attr("order"), false);
    }, function () {
        calendar.tempRemove();
    });
    $(".section").click(function () {
        courseInfo.addSection($(this).attr("order"), true);
    });
    this.refrechConflict();
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
            content += "<div class='section' order=" + i + ">"
                + course.Sections[i].Name + "</div>";
        }
    }
    content += "</div>";

    // Add non-lecture sections
    content += "<div id='courseInfoOtherSections'>";
    for (var i = 0; i < course.Sections.length; i++) {
        if (!course.Sections[i].IsLecture) {
            var item = this.getSectionObject(i, 0);
            content += "<div class='section' order=" + i + ">"
                + course.Sections[i].Name + "</div>";
        }
    }
    content += "</div>";

    // Add instructors
    content += "<div class='courseInfoDescription'><b>Instructor: </b>";
    for (var i = 0; i < course.Sections.length; i++) {
        if (course.Sections[i].IsLecture) {
            if (course.Sections[i].Instructor !== "") {
                content += course.Sections[i].Instructor + " (" + course.Sections[i].Name + ") ";
            }
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
    if (course.BreadthRequirement) {
        content += "<div class='courseInfoDescription'><b>Breadth: </b>" + course.BreadthRequirement + "</div>";
    }
    if (course.DistributionRequirement) {
        content += "<div class='courseInfoDescription'><b> Distribution: </b>" + course.DistributionRequirement + "</div>";
    }
    return content;
}

CourseInfo.prototype.addSection = function (i, permanent) {
    // Avoid duplicate
    if (permanent) {
        calendar.tempRemove();
    }
    if ($(".cal" + this.course.ID + this.course.Sections[i].Name).length > 0) return;

    var meetTimes = this.course.Sections[i].ParsedTime.MeetTimes;
    var ID = this.course.ID + this.course.Sections[i].Name;
    for (var j = 0; j < meetTimes.length; j++) {
        if (permanent) {
            calendar.addItem(this.getSectionObject(i, j, ID));
        } else {
            calendar.tempAdd(this.getSectionObject(i, j, ID));
        }
    }
    if (permanent) {
        this.refrechConflict();
    }
}

CourseInfo.prototype.getSectionObject = function (i, j, ID) {
    var meetTimes = this.course.Sections[i].ParsedTime.MeetTimes;
    var item = {};
    item.Name = this.course.Abbr + ": " + this.course.Name + " " + this.course.Sections[i].Name,
    item.ID = ID,
    item.Day = meetTimes[j].Day,
    item.StartTime = meetTimes[j].Start,
    item.EndTime = meetTimes[j].End,
    item.Semester = this.course.Semester,
    item.Time = this.course.Sections[i].ParsedTime.StringFormat,
    item.Location = this.course.Sections[i].Location,
    item.Order = j;
    return item;
}

CourseInfo.prototype.refrechConflict = function () {
    $sections = $(".section");
    for (var i = 0; i < $sections.length; i++) {
        var order = $($sections[i]).attr("order");
        var section = this.course.Sections[order];
        if (section.ParsedTime.MeetTimes) {
            for (var j = 0; j < section.ParsedTime.MeetTimes.length; j++) {
                var item = this.getSectionObject(order, j, 0);
                if (calendar.isConflicted(item)) {
                    if (!$($sections[i]).hasClass("conflict")) {
                        $($sections[i]).addClass("conflict");
                    }
                } else {
                    if ($($sections[i]).hasClass("conflict")) {
                        $($sections[i]).removeClass("conflict");
                    }
                }
            }
        }
    }
}

// ProgramInfo object
function ProgramInfo(placeholder) {
    this.placeholder = placeholder;
    $(this.placeholder).append("<div id='programSearchResult'></div>");
    $(this.placeholder).append("<div id='programDetail'></div>");
}

ProgramInfo.prototype.getHtml = function () {
    var content = "";
}

ProgramInfo.prototype.loadSearches = function (result) {
    $("#programSearchResult").html(result.Matches);
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