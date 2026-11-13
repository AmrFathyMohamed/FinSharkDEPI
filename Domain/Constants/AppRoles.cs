namespace FinShark.Domain.Constants
{
    // Identity: أسماء الـ Roles في مكان واحد كـ const — بدل ما نكتب "Admin" كـ string في كل حتة ونغلط في حرف.
    // الـ Roles نفسها بتتحط في الداتابيز عن طريق HasData في ApplicationDBContext.
    // const عشان نقدر نستخدمها جوه الـ Attributes زي [Authorize(Roles = AppRoles.SuperAdmin)]
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";   // صلاحيات كاملة
        public const string Admin = "Admin";             // إدارة
        public const string User = "User";               // أي حد بيعمل Register بياخد الـ Role ده

    }
}
