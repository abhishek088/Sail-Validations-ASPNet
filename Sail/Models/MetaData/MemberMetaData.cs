using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sail.Models
{
    [ModelMetadataType(typeof(MemberMetaData))]
    public partial class Member : IValidatableObject
    {
        SailContext _context = new SailContext();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProvinceCode != null)
            {
                if (ProvinceCode.Length != 2)
                    yield return new ValidationResult("Province code cannot be longer than 2 characters", new[] { "ProvinceCode" });
                else
                {
                    var provinceContext = _context.Province.Where(a => a.ProvinceCode == ProvinceCode).FirstOrDefault();
                    if (provinceContext == null)
                        yield return new ValidationResult("Invalid province code", new[] { "ProvinceCode" });
                    else
                        ProvinceCode = ProvinceCode.Trim().ToUpper();
                }
            }

            Regex postalCodeRegex = new Regex((@"^[A-Za-z]{1}[0-9]{1}[A-Za-z]{1}\s{0,1}[0-9]{1}[A-Za-z]{1}[0-9]{1}"), RegexOptions.IgnoreCase);
            if (PostalCode != null)
            {
                if (postalCodeRegex.IsMatch(PostalCode.Trim()))
                {
                    if (!PostalCode.Contains(" "))
                    {
                        PostalCode = PostalCode.Insert(3, " ");
                        PostalCode = PostalCode.Trim().ToUpper();
                    }
                    else
                        PostalCode = PostalCode.Trim().ToUpper();
                }
                else
                    yield return new ValidationResult("Invalid postal code. Postal code must match Canadian postal pattern", new[] { "PostalCode" });
            }

            Regex homePhoneRegex = new Regex(@"^[0-9]{3}-{0,1}[0-9]{3}-{0,1}[0-9]{4}");
            if (HomePhone != null)
            {
                if (homePhoneRegex.IsMatch(HomePhone))
                {
                    if (!HomePhone.Contains('-'))
                    {
                        HomePhone = HomePhone.Insert(3, "-");
                        HomePhone = HomePhone.Insert(7, "-");
                        HomePhone = HomePhone.Trim();
                    }
                }
                else
                    yield return new ValidationResult("Invalid Phone Number Format. It must be in the format : 999-999-9999", new[] { "HomePhone" });
            }

            if (string.IsNullOrEmpty(SpouseFirstName) && string.IsNullOrEmpty(SpouseLastName))
                FullName = LastName.Trim() + ", " + FirstName.Trim();
            else
            {
                if (SpouseLastName == null || SpouseLastName == LastName)
                    FullName = LastName.Trim() + ", " + FirstName.Trim() + " & " + SpouseFirstName.Trim();
                else
                    FullName = LastName.Trim() + ", " + FirstName.Trim() + " & " + SpouseLastName.Trim() + ", " + SpouseFirstName.Trim();
            }

            if (UseCanadaPost)
            {
                if (string.IsNullOrEmpty(Street))
                    yield return new ValidationResult("If Canada post is checked, street name is required", new[] { "Street" });
                if (string.IsNullOrEmpty(City))
                    yield return new ValidationResult("If Canada post is checked, city name is required", new[] { "City" });
            }
            else
            {
                if (string.IsNullOrEmpty(Email))
                    yield return new ValidationResult("If Canada post is not checked, email address is required", new[] { "Email" });
            }

            if (Street != null)
                Street = Street.Trim();

            if (City != null)
                City = City.Trim();

            if (Email != null)
                Email = Email.Trim();

            if (Comment != null)
                Comment = Comment.Trim();

            if (LastName != null)
                LastName = LastName.Trim();

            if (FirstName != null)
                FirstName = FirstName.Trim();

            if (SpouseFirstName != null)
                SpouseFirstName = SpouseFirstName.Trim();

            if (SpouseLastName != null)
                SpouseLastName = SpouseLastName.Trim();

            //determine if editing or creating new
            var memberId = _context.Member.Where(x => x.MemberId == MemberId).FirstOrDefault();
            if (memberId != null)
            {
                //yield error : member id is already on file
            }
            else
            {
                //yield error: member id not on file
            }

            yield return ValidationResult.Success;
        }
    }
    public class MemberMetaData
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        [Required]
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public int? YearJoined { get; set; }
        public string Comment { get; set; }
        public bool TaskExempt { get; set; }
        public bool UseCanadaPost { get; set; }
    }
}
