const amount = { amount: 10 };
function getTests() {
    let token = $('input[name="__RequestVerificationToken"]', $('#testTable')).val();
    let skipAmount = { skipAmount: $('#testTable tr').length - 1 };
    let searchString = { searchString: getUrlParameter("searchString") };
    let dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
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
    if (tests.length === 0 || tests.length !== amount.amount) {
        let button = $('#loadMoreTestsButton');
        button.unbind("click");
        button.prop({ disabled: true });
        return;
    }
    let table = $('#testTable');
    tests.forEach(function (element) {
        let tr = $('<tr/>');
        let testIdTd = $('<td/>', { text: element.id});
        tr.append(testIdTd);
        let testNameRef = $('<a/>', { href: '/TestQuestions/Index/' + element.id, text: element.testName});
        let testNameTd = $('<td/>').append(testNameRef);
        tr.append(testNameTd);
        let formattedDate = new Date(element.creationDate);
        let timeTd = $('<td/>', { text: formattedDate.toLocaleString(), class: 'hidden-xs' });
        tr.append(timeTd);
        let testCodeTd = $('<td/>', { text: element.testCode, class: 'hidden-xs' });
        tr.append(testCodeTd);
        tr.append($('<td/>', { text: element.testDuration.slice(0, -3) }));
        let rateClass = '';
        if (element.testRating > 0)
            rateClass = 'text-success';
        else if (element.testRating < 0)
            rateClass = 'text-danger';
        let td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        let userRef = $('<a/>', { href: '/Profile/Index/' + element.userId, text: element.userName });
        let userTd = $('<td/>', { class: 'hidden-xs'}).append(userRef);
        tr.append(userTd);
        appendControls(tr, element);
        table.append(tr);
    });
}
function appendControls(tr, test) {
    let td = $('<td/>');
    if (test.testCode === null) {
        let editRef = $('<a/>', { href: '/Tests/Edit/' + test.id, text: 'Edit' });
        td.append(editRef);
        td.append('<span> | </span>');
        let deleteRef = $('<a/>', { href: '/Tests/Delete/' + test.id, text: 'Delete' });
        td.append(deleteRef);
        td.append('<span> | </span>');
        let shareTestRef = $('<a/>', { href: '/Tests/CreateCode/' + test.id, text: 'Share test' });
        td.append(shareTestRef);
        td.append('<span> | </span>');
        let validateTestRef = $('<a/>', { href: '/Tests/ValidateTest/' + test.id, text: 'Validate test' });
        td.append(validateTestRef);
        td.append('<span> | </span>');
    }
    else {
        let tryTestRef = $('<a/>', { href: '/TestEngine?code=' + test.testCode, text: 'Try test out' });
        td.append(tryTestRef);
        td.append('<span> | </span>');
        let stopShareRef = $('<a/>', { href: '/Tests/StopSharing/' + test.id, text: 'Stop sharing' });
        td.append(stopShareRef);
        td.append('<span> | </span>');
    }
    let userResultsRef = $('<a/>', { href: '/TestResults/Index/' + test.id, text: 'User results' });
    td.append(userResultsRef);
    td.append('<span> | </span>');
    let detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    td.append(detailsRef);
    tr.append(td);
}

function getReportedTests() {
    let token = $('input[name="__RequestVerificationToken"]', $('#reportedTestTable')).val();
    let skipAmount = { skipAmount: $('#reportedTestTable tr').length - 1 };
    let searchString = { searchString: getUrlParameter("searchString") };
    let dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
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
function appendReportedTests(tests) {
    if (tests.length === 0 || tests.length !== amount.amount) {
        let button = $('#loadMoreReportedTestsButton');
        button.unbind("click");
        button.prop({ disabled: true });
        return;
    }
    var table = $('#reportedTestTable');
    tests.forEach(function (element) {
        let tr = $('<tr/>');
        tr.append($('<td/>', { text: element.id }));
        let testNameRef = $('<a/>', { href: '/TestQuestions/Index/' + element.id, text: element.testName });
        let testNameTd = $('<td/>').append(testNameRef);
        tr.append(testNameTd);
        let userRef = $('<a/>', { href: '/Profile/Index/' + element.userId, text: element.userName });
        let userTd = $('<td/>', { class: 'hidden-xs' }).append(userRef);
        tr.append(userTd);
        tr.append($('<td/>', { text: element.reportAmount }));
        let rateClass = '';
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
    let td = $('<td/>');
    let ignoreRef = $('<a/>', { href: '/Admin/DeleteReports/' + test.id, text: 'Ignore reports' });
    td.append(ignoreRef);
    td.append('<span> | </span>');
    let deleteRef = $('<a/>', { href: '/Tests/Delete/' + test.id, text: 'Delete test' });
    td.append(deleteRef);
    td.append('<span> | </span>');
    if (test.testCode !== null) {
     
        var tryTestRef = $('<a/>', { href: '/TestEngine?code=' + test.testCode, text: 'Try test out' });
        td.append(tryTestRef);
        td.append('<span> | </span>');
    }
    let detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    td.append(detailsRef);
    td.append('<span> | </span>');
    let reportsRef = $('<a/>', { href: '/TestReports/Index/' + test.id, text: 'Reports' });
    td.append(reportsRef);
    tr.append(td);
}

function getUsers() {
    let token = $('input[name="__RequestVerificationToken"]', $('#userTable')).val();
    let skipAmount = { skipAmount: $('#userTable tr').length - 1 };
    let searchString = { searchString: getUrlParameter("searchString") };
    let dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
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
    if (users.length === 0 || users.length !== amount.amount) {
        let button = $('#loadMoreUsersButton');
        button.unbind("click");
        button.prop({ disabled: true });
        return;
    }
    let table = $('#userTable');
    users.forEach(function (element) {
        let tr = $('<tr/>');
        let userRef = $('<a/>', { href: '/Profile/Index/' + element.id, text: element.userName });
        let userTd = $('<td/>').append(userRef);
        tr.append(userTd);
        let nameTd = $('<td/>', { text: element.name, class: 'hidden-xs' });
        tr.append(nameTd);
        let surnameTd = $('<td/>', { text: element.surname, class: 'hidden-xs' });
        tr.append(surnameTd);
        tr.append($('<td/>', { text: element.email }));
        let phoneNumberTd = $('<td/>', { text: element.phoneNumber, class: 'hidden-xs' });
        tr.append(phoneNumberTd);
        appendUsersControlls(tr, element);
        table.append(tr);
    });
}
function appendUsersControlls(tr, user) {
    let td = $('<td/>');
    if (user.role === "Admin") {
        if (user.currentUserUsername !== user.UserName) {
            let removeFromAdminsRef = $('<a/>', { href: '/Admin/RemoveFromAdmins/' + user.id, text: 'Remove from admins' });
            td.append(removeFromAdminsRef);
            td.append('<span> | </span>');
         }
    }
    else {
        if (user.isBanned) {
            let unBanRef = $('<a/>', { href: '/Admin/UnBanUser/' + user.id, text: 'Unban' });
            td.append(unBanRef);
            td.append('<span> | </span>');
        }
        else {
            let banRef = $('<a/>', { href: '/Admin/BanUser/' + user.id, text: 'Ban' });
            td.append(banRef);
            td.append('<span> | </span>');
        }
        if (user.role === "Moderator") {
            if (user.currentUserUsername !== user.userName) {
                let removeFromModeratorsRef = $('<a/>', { href: '/Admin/RemoveFromModerators/' + user.id, text: 'Remove from moderators' });
                td.append(removeFromModeratorsRef);
                td.append('<span> | </span>');
            }
        }
        else {
            let addToModeratorsRef = $('<a/>', { href: '/Admin/AddToModerators/' + user.id, text: 'Add to moderators' });
            td.append(addToModeratorsRef);
            td.append('<span> | </span>');
        }
        let addToAdminsRef = $('<a/>', { href: '/Admin/AddToAdmins/' + user.id, text: 'Add to admins' });
        td.append(addToAdminsRef);
    }
    tr.append(td);
}
$('#loadMoreTestsButton').click(function (e) {
    getTests();
});
$('#loadMoreReportedTestsButton').click(function (e) {
    getReportedTests();
});
$('#loadMoreUsersButton').click(function (e) {
    getUsers();
});