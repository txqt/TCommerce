using T.Library.Model.Users;

namespace T.Library.Model
{
    public class UserListModel
        {
            public int totalUsers { get; set; }
            public int countPages { get; set; }

            public int ITEMS_PER_PAGE { get; set; } = 10;

            public int currentPage { get; set; }

            public List<UserAndRole>? users { get; set; }

        }

        public class UserAndRole : User
        {
            public string? RoleNames { get; set; }
        }


}