$(document).ready(function ()
{
    FillUserRole();
}
);
function FillUserRole() {
    $.ajax({
        url: 'api/ApiUserRole/GetuserRole',
        method: 'GET',
        success: function (response) {
            debugger;
            const users = response.user;
            const roles = response.role;

            $('#userRoles').DataTable({
                data: users,
                columns: [
                    { data: 'userName', title: 'User Name' },
                    { data: 'currentRole', title: 'Current Role' },
                    {
                        data: 'currentRole',
                        title: 'Change Role',
                        render: function (data, type, row) {
                            let select = `<select class="form-select role-dropdown" data-userid="${row.userId}"><option value="0">-- Select Role --</option>`;
                            roles.forEach(role => {
                                const selected = role.name === data ? 'selected' : '';
                                select += `<option value="${role.name}" ${selected}>${role.name}</option>`;
                            });
                            select += '</select>';
                            return select;
                        }
                    }
                ]
            });
        },
        error: function () {
            alert('Failed to load user roles.');
        }
    });
}