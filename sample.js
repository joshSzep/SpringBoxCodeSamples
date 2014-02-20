/// <reference path="/Scripts/jquery-2.1.0.min.js" />
/// <reference path="/Scripts/libs/jqAjaxHelper.js" />
/// <reference path="/Scripts/libs/te.confirm.js" />
/// <reference path="~/Scripts/Libs/te.lib.js" />
"use strict";
var hub;

$(document).ready(function () {
    hub = te.signalr.hub;

    hub.client.StoryBoardUpdated = onStoryBoardUpdated;
    hub.client.StoryBoardDeleted = onStoryBoardDeleted;
    hub.client.StoryBoardAdded = onStoryBoardAdded;

    te.signalr.start().done(function () {
        $('#SaveNewStoryboardButton').on('click', onSaveStoryboardClick);
        $('#StoryboardTable').on('click', '.DeleteButton', onDeleteStoryboardClick);
        $('#StoryboardNameTextBox').on('keypress', SubmitViaEnter);
        $('#CancelNewStoryboardButton').on('click', onCancelClick);
        $('#CreateStoryboardButton').on('click', onCreateStoryboardClick);
    });
});

function onStoryBoardUpdated(response) {
    te.get({ url: '/Storyboard/Table', dataType: 'html' }).done(function (response) {
        $('#StoryboardTable tbody').empty().append(response);
    });
}

function onStoryBoardDeleted(response) {
    te.get({ url: '/Storyboard/Table', dataType: 'html' }).done(function (response) {
        $('#StoryboardTable tbody').empty().append(response);
    });
}

function onStoryBoardAdded() {
    te.get({ url: '/Storyboard/Table', dataType: 'html' }).done(function (response) {
        $('#StoryboardTable tbody').empty().append(response);
    });
}

function SubmitViaEnter(e) {
    return HandleEnter(e, onSaveStoryboardClick);
}

function onDeleteStoryboardClick() {
    var id = $(this).data('id');
    te.confirm('Delete this storyboard?', function () {
        hub.server.deleteStoryboard(id).done(function (response) {
            if (response.Success) {
                te.balloon('success', response.Message, 2000);
            } else {
                te.balloon('alert', response.Message, 2000);
            }
        });
    });
}

function onSaveStoryboardClick() {
    if ($('#StoryboardNameTextBox').val().trim()) {
        hub.server.addStoryboard($('#StoryboardNameTextBox').val()).done(function (response) {
            if (response.Success) {
                window.location.href = "/Storyboard/Details/" + response.StoryboardId;
            } else {
                te.balloon('alert', response.Message, 2000);
            }
        });
    } else {
        $('#StoryboardNameTextBox').focus();
        te.balloon('alert', 'Please enter a name.', 2000);
    }
}

function onCancelClick() {
    $('#new-storyboard-div').hide();
}

function onCreateStoryboardClick() {
    $('#new-storyboard-div').toggle();
}