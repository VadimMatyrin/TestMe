﻿function getTests() {
    var token = $('input[name="__RequestVerificationToken"]', $('#testTable')).val();
    var skipAmount = { skipAmount: $('#testTable tr').length - 1 };
    var amount = { amount: 10 };
    var searchString = { searchString: getUrlParameter("SearchString") };
    var dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken); 
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 
    $.ajax({
        url: "/Tests/GetTestsAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendTests(data);
        },
        error: function () {
   
        }
    });
}
function appendTests(tests) {
    var table = $('#testTable');
    tests.forEach(function (element) {
        var tr = $('<tr/>');
        tr.append($('<td/>', { text: element.id }));
        var testNameRef = $('<a/>', { href: '/TestQuestions/Index/' + element.id, text: element.testName});
        var testNameTd = $('<td/>').append(testNameRef);
        tr.append(testNameTd);
        var formattedDate = new Date(element.creationDate);
        tr.append($('<td/>', { text: formattedDate.toLocaleString() }));
        tr.append($('<td/>', { text: element.testCode }));
        tr.append($('<td/>', { text: element.duration.slice(0, -3) }));
        var rateClass = '';
        if (element.testRating > 0)
            rateClass = 'text-success';
        else if (element.testRating < 0)
            rateClass = 'text-danger';
        var td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        var userRef = $('<a/>', { href: '/Profile/Index/' + element.userId, text: element.userName });
        var userTd = $('<td/>').append(userRef);
        tr.append(userTd);
        appendControls(tr, element);
        table.append(tr);
    });
}
function appendControls(tr, test) {
    var td = $('<td/>');
    if (test.testCode === null) {
        var editRef = $('<a/>', { href: '/Tests/Edit/' + test.id, text: 'Edit' });
        td.append(editRef);
        td.append('<span> | </span>');
        var deleteRef = $('<a/>', { href: '/Tests/Delete/' + test.id, text: 'Delete' });
        td.append(deleteRef);
        td.append('<span> | </span>');
        var shareTestRef = $('<a/>', { href: '/Tests/CreateCode/' + test.id, text: 'Share test' });
        td.append(shareTestRef);
        td.append('<span> | </span>');
        var validateTestRef = $('<a/>', { href: '/Tests/ValidateTest/' + test.id, text: 'Validate test' });
        td.append(validateTestRef);
        td.append('<span> | </span>');
    }
    else {
        var tryTestRef = $('<a/>', { href: '/TestEngine?code=' + test.testCode, text: 'Try test out' });
        td.append(tryTestRef);
        td.append('<span> | </span>');
        var stopShareRef = $('<a/>', { href: '/Tests/StopSharing/' + test.id, text: 'Stop sharing' });
        td.append(stopShareRef);
        td.append('<span> | </span>');
    }
    var userResultsRef = $('<a/>', { href: '/TestResults/Index/' + test.id, text: 'User results' });
    td.append(userResultsRef);
    td.append('<span> | </span>');
    var detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    td.append(detailsRef);
    tr.append(td);
}

function getReportedTests() {
    var token = $('input[name="__RequestVerificationToken"]', $('#reportedTestTable')).val();
    var skipAmount = { skipAmount: $('#reportedTestTable tr').length - 1 };
    var amount = { amount: 1 };
    var searchString = { searchString: getUrlParameter("SearchString") };
    var dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 
    $.ajax({
        url: "/Tests/GetReportedTestsAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendReportedTests(data);
        },
        error: function () {

        }
    });
}
function appendReportedTests(tests)
{
    var table = $('#reportedTestTable');
    tests.forEach(function (element) {
        var tr = $('<tr/>');
        tr.append($('<td/>', { text: element.id }));
        var testNameRef = $('<a/>', { href: '/TestQuestions/Index/' + element.id, text: element.testName });
        var testNameTd = $('<td/>').append(testNameRef);
        tr.append(testNameTd);
        var userRef = $('<a/>', { href: '/Profile/Index/' + element.userId, text: element.userName });
        var userTd = $('<td/>').append(userRef);
        tr.append(userTd);
        tr.append($('<td/>', { text: element.reportAmount }));
        var rateClass = '';
        if (element.testRating > 0)
            rateClass = 'text-success';
        else if (element.testRating < 0)
            rateClass = 'text-danger';
        var td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        appendReportedTestControlls(tr, element);
        table.append(tr);
    });
}
function appendReportedTestControlls(tr, test) {
    var td = $('<td/>');
    var ignoreRef = $('<a/>', { href: '/Admin/DeleteReports/' + test.id, text: 'Ignore reports' });
    td.append(ignoreRef);
    td.append('<span> | </span>');
    var deleteRef = $('<a/>', { href: '/Tests/Delete/' + test.id, text: 'Delete test' });
    td.append(deleteRef);
    td.append('<span> | </span>');
    if (test.testCode !== null) {
     
        var tryTestRef = $('<a/>', { href: '/TestEngine?code=' + test.testCode, text: 'Try test out' });
        td.append(tryTestRef);
        td.append('<span> | </span>');
    }
    var detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    td.append(detailsRef);
    td.append('<span> | </span>');
    var reportsRef = $('<a/>', { href: '/TestReports/Index/' + test.id, text: 'Reports' });
    td.append(reportsRef);
    tr.append(td);
}

function getUsers() {
    var token = $('input[name="__RequestVerificationToken"]', $('#userTable')).val();
    var skipAmount = { skipAmount: $('#userTable tr').length - 1 };
    var amount = { amount: 1 };
    var searchString = { searchString: getUrlParameter("SearchString") };
    var dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 
    $.ajax({
        url: "/Admin/GetUsersAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendUsers(data);
        },
        error: function () {

        }
    });
}

function appendUsers(users) {
    var table = $('#userTable');
    users.forEach(function (element) {
        var tr = $('<tr/>');
        var userRef = $('<a/>', { href: '/Profile/Index/' + element.userId, text: element.userName });
        var userTd = $('<td/>').append(userRef);
        tr.append(userTd);
        tr.append($('<td/>', { text: element.name }));
        tr.append($('<td/>', { text: element.surname }));
        tr.append($('<td/>', { text: element.email }));
        tr.append($('<td/>', { text: element.phoneNumber }));
        appendUsersControlls(tr, element);
        table.append(tr);
    });
}
function appendUsersControlls(tr, user) {
    var td = $('<td/>');
    if (user.role === "Admin") {
        if (user.currentUserUsername !== user.UserName) {
            var removeFromAdminsRef = $('<a/>', { href: '/Admin/RemoveFromAdmins/' + user.id, text: 'Remove from admins' });
            td.append(removeFromAdminsRef);
            td.append('<span> | </span>');
         }
    }
    else {
        if (user.role === "Moderator") {
            if (user.currentUserUsername !== user.userName) {
                var removeFromModeratorsRef = $('<a/>', { href: '/Admin/RemoveFromModerators/' + user.id, text: 'Remove from moderators' });
                td.append(removeFromModeratorsRef);
                td.append('<span> | </span>');
            }
        }
        else {
            var addToModeratorsRef = $('<a/>', { href: '/Admin/AddToModerators/' + user.id, text: 'Add to moderators' });
            td.append(addToModeratorsRef);
            td.append('<span> | </span>');
        }
        var addToAdminsRef = $('<a/>', { href: '/Admin/AddToAdmins/' + user.id, text: 'Add to admins' });
        td.append(addToAdminsRef);
    }
    tr.append(td);
}