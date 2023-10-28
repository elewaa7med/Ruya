using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SmartAdmin.WebUI.Extensions
{
    public static class EnumExtention
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }

    public enum Grade
    {
        [Display(Name = "PREKG")]
        PREKG = 1,
        [Display(Name = "KG 1")]
        KG1 = 2,
        [Display(Name = "KG 2")]
        KG2 = 3,
        [Display(Name = "KG 3")]
        KG3 = 4,
        [Display(Name = "Grade 1")]
        Grade1 = 5,
        [Display(Name = "Grade 2")]
        Grade2 = 6,
        [Display(Name = "Grade 3")]
        Grade3 = 7,
        [Display(Name = "Grade 4")]
        Grade4 = 8,
        [Display(Name = "Grade 5")]
        Grade5 = 9,
    }

    public enum UpcomingSchoolYear
    {
        [Display(Name = "PREKG")]
        PREKG = 1,
        [Display(Name = "KG 1")]
        KG1 = 2,
        [Display(Name = "KG 2")]
        KG2 = 3,
        [Display(Name = "KG 3")]
        KG3 = 4,
        [Display(Name = "Grade 1")]
        Grade1 = 5,
        [Display(Name = "Grade 2")]
        Grade2 = 6,
        [Display(Name = "Grade 3")]
        Grade3 = 7,
        [Display(Name = "Grade 4")]
        Grade4 = 8,
        [Display(Name = "Grade 5")]
        Grade5 = 9,
        [Display(Name = "Grade 6")]
        Grade6 = 10
    }


    public enum SchoolSystem
    {
        [Display(Name = "National School")]
        National_school = 1,
        [Display(Name = "Internation School In Ksa")]
        international_school_in_Ksa = 2,
        [Display(Name = "Abroad Schooling")]
        Abroad_schooling = 3,
        [Display(Name = "Governed Public Schools")]
        Governed_public_schools = 4
    }

    public enum TuitionPaymentMethods
    {
        [Display(Name = "Company will cover full tuition fees")]
        company = 1,
        [Display(Name = "Parents will cover full tuition fees")]
        parents = 2,
        [Display(Name = "Company will partly subsidize tuition fees")]
        companyPartialy = 3
    }

    public enum boolean
    {
        No = 1,
        Yes = 2,
    }

    public enum Religion
    {
        Muslim = 1,
        Christian = 2,
        Jewish = 3

    }

    public enum Status
    {

        [Display(Name = "Pending")]
        Pending = 1,
        [Display(Name = "Stage 2")]
        Stage2,
        [Display(Name = "Accepted with finical file")]
        Acceptedfinical ,
        [Display(Name = "Rejected")]
        Rejected,
        [Display(Name = "Accepeted on Waiting list")]
        AcceptedWaiting,
        [Display(Name = "Accepted with conditions with financial claim")]
        Acceptedconditions,
        [Display(Name = "Accepted with recommendation with financial claim")]
        Acceptedrecommendation,
        
    }

    public enum Nationality
    {
        Afghan = 1,
        Albanian,
        Algerian,
        American,
        Andorran,
        Angolan,
        Antiguans,
        Argentinean,
        Armenian,
        Australian,
        Austrian,
        Azerbaijani,
        Bahamian,
        Bahraini,
        Bangladeshi,
        Barbadian,
        Barbudans,
        Batswana,
        Belarusian,
        Belgian,
        Belizean,
        Beninese,
        Bhutanese,
        Bolivian,
        Bosnian,
        Brazilian,
        British,
        Bruneian,
        Bulgarian,
        Burkinabe,
        Burmese,
        Burundian,
        Cambodian,
        Cameroonian,
        Canadian,
        Chadian,
        Chilean,
        Chinese,
        Colombian,
        Comoran,
        Congolese,
        Croatian,
        Cuban,
        Cypriot,
        Czech,
        Danish,
        Djibouti,
        Dominican,
        Dutch,
        Ecuadorean,
        Egyptian,
        Emirian,
        Eritrean,
        Estonian,
        Ethiopian,
        Fijian,
        Filipino,
        Finnish,
        French,
        Gabonese,
        Gambian,
        Georgian,
        German,
        Ghanaian,
        Greek,
        Grenadian,
        Guatemalan,
        Guyanese,
        Haitian,
        Herzegovinian,
        Honduran,
        Hungarian,
        Icelander,
        Indian,
        Indonesian,
        Iranian,
        Iraqi,
        Irish,
        Israeli,
        Italian,
        Ivorian,
        Jamaican,
        Japanese,
        Jordanian,
        Kazakhstani,
        Kenyan,
        Kuwaiti,
        Kyrgyz,
        Laotian,
        Latvian,
        Lebanese,
        Liberian,
        Libyan,
        Liechtensteiner,
        Lithuanian,
        Luxembourger,
        Macedonian,
        Malagasy,
        Malawian,
        Malaysian,
        Maldivan,
        Malian,
        Maltese,
        Marshallese,
        Mauritanian,
        Mauritian,
        Mexican,
        Micronesian,
        Moldovan,
        Monacan,
        Mongolian,
        Moroccan,
        Mosotho,
        Motswana,
        Mozambican,
        Namibian,
        Nauruan,
        Nepali,
        Nicaraguan,
        Nigerian,
        Nigerien,
        Norwegian,
        Omani,
        Pakistani,
        Palauan,
        Panamanian,
        Paraguayan,
        Peruvian,
        Polish,
        Portuguese,
        Qatari,
        Romanian,
        Russian,
        Rwandan,
        Saint ,
        Salvadoran,
        Samoan,
        Saudi,
        Scottish,
        Senegalese,
        Serbian,
        Seychellois,
        Singaporean,
        Slovakian,
        Slovenian,
        Somali,
        Spanish,
        Sudanese,
        Surinamer,
        Swazi,
        Swedish,
        Swiss,
        Syrian,
        Taiwanese,
        Tajik,
        Tanzanian,
        Thai,
        Togolese,
        Tongan,
        Tunisian,
        Turkish,
        Tuvaluan,
        Ugandan,
        Ukrainian,
        Uruguayan,
        Uzbekistani,
        Venezuelan,
        Vietnamese,
        Welsh,
        Yemenite,
        Zambian,
        Zimbabwean
    }


}

