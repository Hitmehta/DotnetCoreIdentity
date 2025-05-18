let roles = [];  // to hold all roles fetched from server
let currentUserId = null;
$(document).ready(function () {
    FillUserRole();

    // When clicking Edit button
    $(document).on('click', '.edit-role-btn', function () {
        currentUserId = $(this).data('userid');
        const currentRole = $(this).data('currentrole');

        // Clear previous options
        $('#roleSelect').empty();

        // Populate dropdown options
        roles.forEach(role => {
            const selected = role.name === currentRole ? 'selected' : '';
            $('#roleSelect').append(`<option value="${role.name}" ${selected}>${role.name}</option>`);
        });
    });

    // Handle Save changes button click
    $('#saveRoleBtn').on('click', function () {
        const selectedRole = $('#roleSelect').val();

        if (!currentUserId) {
            alert('User not selected.');
            return;
        }

        var formData = new FormData();
        formData.append("userID", currentUserId);
        formData.append("roleName", selectedRole);
        $.ajax({
            url: '/api/ApiUserRole/ChangeUserRole',
            method: 'POST',
            data: formData,
            processData: false, // important!
            contentType: false,
            success: function () {
                $('#EditRoleModel').modal('hide');
                window.location.reload();
            },
            error: function () {
                alert('Failed to update role.');
            }
        });
    });
});
function FillUserRole() {
    $.ajax({
        url: 'api/ApiUserRole/GetuserRole',
        method: 'GET',
        success: function (response) {
            const users = response.user;
            roles = response.role;  // store roles globally

            if ($.fn.DataTable.isDataTable('#userRoles')) {
                $('#userRoles').DataTable().clear().rows.add(users).draw();
            } else {
                $('#userRoles').DataTable({
                    data: users,
                    columns: [
                        { data: 'userName', title: 'User Name' },
                        { data: 'currentRole', title: 'Current Role' },
                        {
                            data: null,
                            render: function (data, type, row) {
                                return `<button class="btn btn-primary edit-role-btn" data-userid="${row.userId}" data-currentrole="${row.currentRole}" data-bs-toggle="modal" data-bs-target="#EditRoleModel">Edit</button>`;
                            }
                        }
                    ]
                });
            }
        },
        error: function () {
            alert('Failed to load user roles.');
        }
    });
}


