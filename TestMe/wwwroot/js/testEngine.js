﻿var testIds = [];
function finishTestButton() {
    $('#finishTestModal').modal('toggle');
    $('#finishTestModal').on('hidden.bs.modal', function (e) {
        finishTest();
    });
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
            configureForTheFirstQuestion(data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function checkAnswerClick() {
    if ($('input[name="answer"]:checked').length === 0)
        return;
    checkAnswer();
}
function checkAnswer() {
    var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
    var checkedArray = new Array();
    $('input[name="answer"]:checked').each(function () {
        checkedArray.push(this.value);
    });

    var myData = { checkedIds: checkedArray };
        questionId = $("#testQuestionFieldSet").data('id');
    myData = $.extend(myData, { 'questionId': questionId });
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
    $.ajax({
        url: "/TestEngine/CheckAnswer",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            showCorrectAnswer(checkedArray);
        },
        error: function () {
            //$("#questionBlock").append('<h5> Internal error. Try again</h5>');
        }
    });

}
function getIfAnswered() {
        var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
        var questionId = { questionId: $("#testQuestionFieldSet").data("id") };
        var dataWithAntiforgeryToken = $.extend(questionId, { '__RequestVerificationToken': token });
        $.ajax({
            url: "/TestEngine/GetIfAnswered",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                if(data !== "notAnswered")
                    showCorrectAnswer(data);
            },
            error: function () {
                //$("#questionBlock").append('<h5> Internal error. Try again</h5>');
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
            getIfAnswered();
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
function displayQuestionNav(questionIds) {
    testIds = questionIds.slice();
    setNavButtonsQuestIds(testIds[0]);
    $('#mainContainer').data('questAmount', questionIds.length);
    for (let i = 0; i < questionIds.length; i++) {
        let button = $('<button />', {
            type: 'button',
            class: 'btn btn-info',
            value: questionIds[i],
            text: (i + 1),
            click: function (event) {
                event.preventDefault();
                var prevButton = $('#questions > div > button.btn-primary');
                if (!prevButton.hasClass('btn-default'))
                    prevButton.addClass('btn-info');

                prevButton.removeClass('btn-primary');
                $(this).removeClass('btn-info');
                $(this).addClass('btn-primary');
                getQuestion($(this).attr('value'));
                return false;
            }
        });
        $('#questions > div > button').first().removeClass('btn-info');
        $('#questions > div > button').first().addClass('btn-primary');
        $('<div />', { class: 'col-xs-1' }).append(button).appendTo('div#questions');
    }
    $('#questions > button:first-child').addClass('btn-primary');
}
function configureForTheFirstQuestion(question) {
    getQuestionsId();
    $('#questionBlock').show();
    $('#startTestElem').remove();
    appendQuestion(question);
    getUserName();
    getEndTime();
    $('#nextQuestionButton').click(function (event) {
        var testQuestionId = $(this).data('nextQuestionId');
        $("button[value='" + testQuestionId + "']").click();
        return false;
    });
    $('#prevQuestionButton').click(function (event) {
        var testQuestionId = $(this).data('prevQuestionId');
        $("button[value='" + testQuestionId + "']").click();
        return false;
    });


}
function setNavButtonsQuestIds(questionId) {
    var currIdIndex = testIds.indexOf(questionId);
    if (currIdIndex - 1 < 0) {
        $('#prevQuestionButton').prop({ disabled: true });
    }
    else {
        $('#prevQuestionButton').prop({ disabled: false });
        $('#prevQuestionButton').data('prevQuestionId', testIds[currIdIndex - 1]);
    }
    if (currIdIndex + 1 >= testIds.length) {
        $('#nextQuestionButton').prop({ disabled: true });
    }
    else {
        $('#nextQuestionButton').prop({ disabled: false });
        $('#nextQuestionButton').data('nextQuestionId', testIds[currIdIndex + 1]);
    }
}
function appendQuestion(question) {
    $('#testQuestionFieldSet div').remove();
    $('#answerButton').removeClass('btn-default');
    $('#answerButton').addClass('btn-success');
    $('#answerButton').text('Answer');
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
            var image = $('<img />', { src: '/uploads/answerPics/' + element.imageName, height: "200",class: 'answerImage' });
            image.appendTo(div);
            image.click(function () {
                if ($(this).hasClass('max')) {
                    $(this).animate({ height: 200 }, 200).removeClass('max');
                } else {
                    $(this).animate({ height: 600 }, 200).addClass('max');
                }
            });
        }
        div.appendTo('#testQuestionFieldSet');
        $("input[type='checkbox']").change(function () {
            var checkButton = $('#answerButton');
            if (checkButton.hasClass('btn-default')) {
                $('#answerButton').removeClass('btn-default');
                $('#answerButton').addClass('btn-primary');
            }
        });
    });
    setNavButtonsQuestIds(question.id);
}
function showCorrectAnswer(userAnswers) {
    userAnswers.forEach(function (element) {
        $("input[type=checkbox][value='" + element + "']").prop('checked', true);
    });

    var navButton = $('button[value="' + $('#testQuestionFieldSet').data('id') + '"]');
    navButton.removeClass('btn-primary');
    var div = $('<div />', { id: 'answerMessage', class: 'text-center text-primary col-md-offset-3 col-md-2 col-xs-8 col-xs-offset-2' });
    navButton.addClass('btn-default');
    var h3 = $('<h3 />', { class: 'text-center', text: 'Answered' });
    $('#answerButton').text('Change answer');
    $('#answerButton').removeClass('btn-primary');
    $('#answerButton').removeClass('btn-success');
    $('#answerButton').addClass('btn-default');
    div.append(h3);

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
            if ('isRated' in data) {
                if (data.isRated)
                    $('#likeButton').addClass('btn-primary');
                else
                    $('#dislikeButton').addClass('btn-primary');
            }
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function rateTest(mark) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var myData = { mark: mark };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

    $.ajax({
        url: "/TestEngine/RateFinishedTestAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            if (data) {
                $('#dislikeButton').removeClass('btn-primary');
                $('#likeButton').addClass('btn-primary');
            }
            else
            {
                $('#likeButton').removeClass('btn-primary');
                $('#dislikeButton').addClass('btn-primary');
            }
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function showResult(data) {
    $('#questionBlock').remove();
    var link = $('<a />', { href: "/TestResults/Index/" + data.testId, text: "Other users results" });
    $('#mainContainer').append('<h1> Your score: ' + data.score + ' out of ' + $('#mainContainer').data('questAmount') + '</h1>');
    showRateButtons();
    $('#mainContainer').append(link);

}
function showRateButtons() {
    var likeButton = $('<button/>', { type: 'button', class: 'btn btn-default btn-sm', id: 'likeButton' });
    likeButton.click(function () {
        rateTest(true);
    });
    $('<span/>', { class: 'glyphicon glyphicon-thumbs-up', test: 'Like' }).appendTo(likeButton);
    var dislikeButton = $('<button/>', { type: 'button', class: 'btn btn-default btn-sm', id: 'dislikeButton' });
    dislikeButton.click(function () {
        rateTest(false);
    });
    $('<span/>', { class: 'glyphicon glyphicon-thumbs-down', test: 'Dislike' }).appendTo(dislikeButton);
    $('#mainContainer').append('<p>Rate this test:</p>')
    $('#mainContainer').append(likeButton);
    $('#mainContainer').append(dislikeButton);
    $('#mainContainer').append('<br/>');
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
            clearInterval(x);
            finishTest();
        }
    }, 1000);
}
$("#questionBlock").hide();

    
