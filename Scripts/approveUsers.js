$(function() {

    function ApproveUsers() {};

    ApproveUsers.prototype.initialize = function() {
        this.setupListeners();
    };

    ApproveUsers.prototype.setupListeners = function() {
        $('.usersToApproveList').on('click', '.good', this.approveUser.bind(this));
        $('.usersToApproveList').on('click', '.bad', this.disapproveUser.bind(this));
    };

    ApproveUsers.prototype.approveUser = function(e) {
        var icon = e.target;
        var id = $(icon).parent().data('employee-id');

        $.ajax('/Account/ApproveUser', {
            type: 'POST',
            data: {
                employeeId: id
            },
            success: function () {
                $(icon).parent().fadeOut();
            }
        });
    };

    ApproveUsers.prototype.disapproveUser = function (e) {
        var icon = e.target;
        var id = $(icon).parent().data('employee-id');

        $.ajax('/Account/DisapproveUser', {
            type: 'POST',
            data: {
                employeeId: id
            },
            success: function() {
                $(icon).parent().fadeOut();
            }
        });
    };
});
