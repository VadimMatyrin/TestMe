﻿function finishTestButton() {
    $('#finishTestModal').modal('toggle');
    $('#finishTestModal').on('hidden.bs.modal', function (e) {
        finishTest();
    })
}
function getUserName() {
    var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
    var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };
    $.ajax({
        url: "/TestEngine/GetUserName",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            $('#questionText h5').text('Username: ' + data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function startTest() {
    var token = $('input[name="__RequestVerificationToken"]', $('#startTestElem')).val();
    var myData = { code: $("#testCode").val() };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

    $.ajax({
        url: "/TestEngine/StartTest",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            ConfigureForTheFirstQuestion(data);
            getQuestionsId();
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function checkAnswerClick() {
    if ($('input[name="answer"]:checked').length == 0)
        return;
    CheckAnswer();
}
function prevButtonClick() {
    var questionId = $("#testQuestionFieldSet").data("id");
    if ($('#questions button').first().val() == questionId)
        return;
    getPrevQuestion();
}
function nextButtonClick() {
    var questionId = $("#testQuestionFieldSet").data("id");
    if ($('#questions button').last().val() == questionId)
        return;
    getNextQuestion();
}
function CheckAnswer(questionId, checkedArray) {
    var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
    if (checkedArray === undefined) {
        checkedArray = new Array();
        $('input[name="answer"]:checked').each(function () {
            checkedArray.push(this.value);
        });
    }

    var myData = { checkedIds: checkedArray };
    if (typeof questionId !== 'number') {
        questionId = $("#testQuestionFieldSet").data('id');
    }
    var testCode = $("#testQuestionFieldSet").data("testCode");
    myData = $.extend(myData, { 'questionId': questionId });
    myData = $.extend(myData, { 'testCode': testCode });
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
    $.ajax({
        url: "/TestEngine/CheckAnswer",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            showCorrectAnswer(data, checkedArray);
        },
        error: function () {
            //$("#questionBlock").append('<h5> Internal error. Try again</h5>');
        }
    });

}
function getNextQuestion() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var myData = { questionId: $("#testQuestionFieldSet").data("id") };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

    $.ajax({
        url: "/TestEngine/GetNextQuestion",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendQuestion(data);
            getIfAlreadyAswered(data);
            changeSelectedBitton(data.id);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function getPrevQuestion() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var myData = { questionId: $("#testQuestionFieldSet").data("id") };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

    $.ajax({
        url: "/TestEngine/GetPrevQuestion",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendQuestion(data);
            getIfAlreadyAswered(data);
            changeSelectedBitton(data.id);
            //$("#prevQuestionButton").prop('disabled', false);
        },
        error: function () {
            // $("#questionForm").empty();
        }
    });
}
function getQuestion(questionId) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var myData = { questionId: questionId };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

    $.ajax({
        url: "/TestEngine/GetQuestion",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendQuestion(data);
            getIfAlreadyAswered(data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function getIfAlreadyAswered(questionAnswers) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var myData = { questionId: $("#testQuestionFieldSet").data("id") };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

    $.ajax({
        url: "/TestEngine/GetIfAlreadyAnswered",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            if (data !== "notAnswered")
                CheckAnswer(questionAnswers, data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function getQuestionsId() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };

    $.ajax({
        url: "/TestEngine/GetQuestionsIds",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            displayQuestionNav(data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function getEndTime() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };

    $.ajax({
        url: "/TestEngine/GetEndTime",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            startTimer(data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function changeSelectedBitton(questionId) {
    var prevButton = $('#questions > div > button.btn-primary');
    if (!prevButton.hasClass('btn-success') && !prevButton.hasClass('btn-danger'))
        prevButton.addClass('btn-info');

    prevButton.removeClass('btn-primary');
    $('#questions > div > button[value="' + questionId + '"]').removeClass('btn-info');
    $('#questions > div > button[value="' + questionId + '"]').addClass('btn-primary');

}
function displayQuestionNav(questionIds) {
    $('#mainContainer').data('questAmount', questionIds.length);
    for (let i = 0; i < questionIds.length; i++) {
        let button = $('<button />', {
            type: 'button',
            class: 'btn btn-info',
            value: questionIds[i],
            text: (i + 1),
            click: function () {
                var prevButton = $('#questions > div > button.btn-primary');
                if (!prevButton.hasClass('btn-success') && !prevButton.hasClass('btn-danger'))
                    prevButton.addClass('btn-info');

                prevButton.removeClass('btn-primary');
                $(this).removeClass('btn-info');
                $(this).addClass('btn-primary');
                getQuestion($(this).attr('value'));
            }
        })
        changeSelectedBitton(questionIds[0]);
        $('<div />', { class: 'col-xs-1' }).append(button).appendTo('div#questions');
    }
    $('#questions > button:first-child').addClass('btn-primary');
}
        function ConfigureForTheFirstQuestion(question) {
        $('#questionBlock').show();
    $('#startTestElem').remove();
    appendQuestion(question);
    getUserName();
    getEndTime();
}
function appendQuestion(question) {
    $('#testQuestionFieldSet div').remove();
    $('#answerMessage').remove();
    $('#answerButton').show();
    $('#questionText h1').text(question.questionText);
    $('#testQuestionFieldSet').data('testCode', question.test.testCode);
    $('#testQuestionFieldSet').data('id', question.id);
    question.testAnswers.forEach(function (element) {
        var input = $('<input />', { type: 'checkbox', name: 'answer', value: element.id });
        var label = $('<label />', { text: element.answerText });
        var div = $('<div />', { class: "questionAnswer" });
        input.appendTo(div);
        label.appendTo(div);
        $('<br>').appendTo(div);
        if (element.imageName) {
            var image = $('<img />', { src: '/uploads/answerPics/' + element.imageName, height: "200" });
            image.appendTo(div);
        }
        div.appendTo('#testQuestionFieldSet');
    });
}
function showCorrectAnswer(questionAnswers, userAnswers) {
    userAnswers.forEach(function (element) {
        $("input[type=checkbox][value='" + element + "']").closest("div").css("color", "red");
        $("input[type=checkbox][value='" + element + "']").prop('checked', true);
    });

    questionAnswers.forEach(function (element) {
        $("input[type=checkbox][value='" + element + "']").closest('div').css('color', 'green');
    });
    var navButton = $('button[value="' + $('#testQuestionFieldSet').data('id') + '"]');
    navButton.removeClass('btn-info');
    var div = $('<div />', { id: 'answerMessage', class: 'text-center text-primary col-md-offset-3 col-md-2 col-xs-8 col-xs-offset-2' });
    if (isAllCorrect(questionAnswers, userAnswers)) {
        var h3 = $('<h3 />', { class: 'text-center text-success isCorrectText ', text: 'Correct' });
        navButton.addClass('btn-success');
    }
    else {
        var h3 = $('<h3 />', { class: 'text-center text-danger isCorrectText', text: 'Incorrect' });
        navButton.addClass('btn-danger');
    }
    $('#answerButton').hide();
    $('#answerMessage').remove();
    div.append(h3);
    div.insertAfter($('#prevQuestionButton'));
    $('input[name="answer"]').each(function () {
        $(this).prop('disabled', true);
    });

}
function isAllCorrect(questionAnswers, userAnswered) {
    if (questionAnswers.length !== userAnswered.length)
        return false;
    for (var i = questionAnswers.length; i--;) {
        if (questionAnswers[i] != userAnswered[i])
            return false;
    }

    return true;
}
function finishTest() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };

    $.ajax({
        url: "/TestEngine/FinishTest",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            showResult(data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
        function showResult(data) {
            $('#questionBlock').remove();
        var link = $('<a />', {href: "/Tests/UserResults/" + data.testId, text: "Other users results" });
            $('#mainContainer').append('<h1> Your score: ' + data.score + ' out of ' + $('#mainContainer').data('questAmount') + '</h1>');
        $('#mainContainer').append(link);

    }
function startTimer(date) {
    var countDownDate = new Date(date).getTime();

    var x = setInterval(function () {

        var now = new Date().getTime();

        var distance = countDownDate - now;

        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        if (minutes < 10)
            minutes = "0" + minutes;
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);
        if (seconds < 10)
            seconds = "0" + seconds;
        $('#timer').text(hours + ":"
            + minutes + ":" + seconds + "s ");

        if (distance <= 500) {
            finishTest();
        }
    }, 1000);
}
$("#questionBlock").hide();

    