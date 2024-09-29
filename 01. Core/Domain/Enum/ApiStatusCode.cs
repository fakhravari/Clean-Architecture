using System.ComponentModel.DataAnnotations;

namespace Domain.Enum;

public enum ApiStatusCode
{
    [Display(Name = "عملیات با موفقیت انجام شد")]
    Success = 200,

    [Display(Name = "خطایی در سرور رخ داده است")]
    ServerError = 500,

    [Display(Name = "پارامتر های ارسالی معتبر نیستند")]
    BadRequest = 400,

    [Display(Name = "یافت نشد")]
    NotFound = 404,

    [Display(Name = "لیست خالی است")]
    ListEmpty = 204,

    [Display(Name = "خطایی در پردازش رخ داد")]
    LogicError = 422,

    [Display(Name = "خطای احراز هویت")]
    UnAuthorized = 401
}
