var searchStringTemp = "";
var onAnimation = false;
var calendar;

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

    calendar.addItem({ Name: "MAT292H1F", ID: 0, Day: 1, StartTime: 32, EndTime: 36 });
    calendar.addItem({ Name: "MAT292H1F", ID: 1, Day: 2, StartTime: 34, EndTime: 40 });
    calendar.addItem({ Name: "MAT292H1F", ID: 2, Day: 5, StartTime: 40, EndTime: 56 });


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
    $.getJSON("api/search?id=" + parseInt(id.substring(1)), function (course) { showCourseInfo(course, target); });
}

function showResults(result) {

    var content = "";

    function addCollectionToContent(collection, name, title, collapsed) {
        if (collection) {
            content += "<div class='matchCollection" + (collapsed ? " collapsed" : "") +
                "'><div class='matchCollectionTitle'>" + title + "</div><ul id='" + name + "'>" + collection + "</ul></div>";
        }
    }

    addCollectionToContent(result.CodeNameMatches, "codeNameMatches", "", false);
    addCollectionToContent(result.DescriptionMatches, "desMatches", "Relevant Description", true);
    addCollectionToContent(result.DepartmentMatches, "depMatches", "Relevant Department", true);
    addCollectionToContent(result.PrerequisiteMatches, "preqMatches", "Relevant Prerequisite", true);
    addCollectionToContent(result.RawMatches, "rawMatches", "Other Relevant Courses", true);

    $("#resultView").html(content);
    $(".courseResult").click(function (e) {
        searchCourseInfo($(e.target).attr("id"), $(e.target));
    });

    $(".matchCollectionTitle").click(function (e) {
        toggleMatchCollection($(e.target));
    });
    $(".matchCollection.collapsed").height(20);
}

function ResultView(placeholder, result) {
    this.placeholder = placeholder;
    this.result = result;
    //this.codeNameMatches = null;
    //this.descriptionMatches = null;
    //this.departmentMatches = null;
    //this.prerequisiteMatches = null;
    //this.rawMatches = null;
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

    addCollection(result.CodeNameMatches, "codeNameMatches", "", false);
    addCollection(result.DescriptionMatches, "desMatches", "Relevant Description", true);
    addCollection(result.DepartmentMatches, "depMatches", "Relevant Department", true);
    addCollection(result.PrerequisiteMatches, "preqMatches", "Relevant Prerequisite", true);
    addCollection(result.RawMatches, "rawMatches", "Other Relevant Courses", true);

    $(".matchCollection.collapsed").height(20);
}

function MatchCollection(placeholder, name, collection) {
    this.placeholder = placeholder;
    this.collection = this.rearrangeCollection(collection);

    var content = "<div class='matchCollectionTitle'>" + name + "</div><ul>" + collection + "</ul>";

    $(this.placeholder).html(content);
    $(this.placeholder).children(".matchCollectionTitle").click(this.onTitleClick);
    $(this.placeholder).children(".courseResult").click(function (e) {
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

// TODO make course info page an object
function showCourseInfo(course, target) {
    var content;
    var courseInfo = $("#courseInfo");
    content = "<div id='courseInfoTitle'><b>" + course.Abbr + ": " + course.Name + "</b></div>";
    if (course.Description) {
        content += "<div class='courseInfoDescription'>" + course.Description + "</div>";
    }
    content += "<div id='courseInfoSections'><b>Sections:</b>";
    for (var i = 0; i < course.Sections.length; i++) {
        content += " " + course.Sections[i].Name;
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

    courseInfo.html(content);
    var top = target.offset()["top"];
    if (top + courseInfo.height() > window.innerHeight - 10) {
        courseInfo.css("top", top - courseInfo.height() + 20);
    } else {
        courseInfo.css("top", top);
    }
    courseInfo.show();
}

function toggleMatchCollection(target) {
    if (target.parent().hasClass("collapsed")) {
        target.parent().animate({ height: target.next().children().length * 24 + 20 }, 200, function () {

            var scrollTop = target.parent().parent().scrollTop();
            var top = target.position()["top"];

            if (target.parent().height() > $("#resultView").height()) {
                target.parent().parent().scrollTop(scrollTop + top - 40);
            } else {
                target.parent().parent().scrollTop(scrollTop + top - $("#resultView").height() + target.parent().height());
            }
        });
        target.parent().removeClass("collapsed");
    } else {
        target.parent().animate({ height: 20 }, 200);
        target.parent().addClass("collapsed");
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