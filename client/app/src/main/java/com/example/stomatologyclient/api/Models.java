package com.example.stomatologyclient.api;

import com.example.stomatologyclient.models.NamedModel;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import retrofit2.http.Body;

/**
 * Список моделей для Retrofit.
 * Наследование от NamedModel  нужно, чтобы
 * пихать в адаптер.
 *
 * Это универсальный адаптер, так что немного говнокодно -
 * NamedModel содержит поля на все случаи жизини:
 *  - картинку
 *  - название
 *  - стоимость
 */
public class Models
{
    static DateFormat df = new SimpleDateFormat("dd.MM.yyyy");


    //Преобразует ФИО в строку, исключая отчество, если его нет
    private static String convertFIOToName(PersonInfo personInfo)
    {
        StringBuilder sb = new StringBuilder();
        sb.append(personInfo.Surname).append(" ")
                .append(personInfo.Name);

        if(personInfo.Middlename!=null)
            sb.append(" ").append(personInfo.Middlename);

        return  sb.toString();
    }

    /**
     * Чтобы получать Id новой сущности
     */
    public static class PutResponse
    {
        public int Id;
    }

    /**
     * Категория
     */
    public static class Category  extends NamedModel
    {
        public String Name;
        public String Description;
        public List<Subcategory> Subcategories = new ArrayList<Subcategory>();

        @Override
        public String Name() {
            return Name;
        }
    }

    /**
     * Подкатегория
     */
    public static class Subcategory extends NamedModel
    {
        public String Name;
        public Integer CategoryId;
        public List<Procedure> Procedures = new ArrayList<Procedure>();

        @Override
        public String Name() {
            return Name;
        }
    }

    /**
     * Процедура
     */
    public static class Procedure extends NamedModel
    {
        public String Name;
        public String Description;
        public Integer SubcategoryId;
        public float Cost;

        @Override
        public String Name() {
            return Name;
        }

        @Override
        public float Cost()
        {
            return  Cost;
        }
    }

    /**
     * Инфа о человеке
     */
    public static class PersonInfo
    {

        public String Surname;
        public String Name;
        public String Middlename;
        public Integer Id;

    }

    /**
     * Инфа о клинике
     */
    public static class ClinicInfo {

        public String Name;
        public String PhoneNumber;
        public String Email;
        public String Adress;
        public Integer Id;

    }

    /**
     * Зубной техник
     */
    public static class DentalTechnican extends NamedModel
    {

        public PersonInfo PersonInfo;
        public Integer ApplicationUserId;
        public Integer PersonInfoId;


        @Override
        public String Name() {
            return Models.convertFIOToName(PersonInfo);
        }
    }

    /**
     * Врач
     */
    public static class Doctor extends NamedModel
    {

        public PersonInfo PersonInfo;
        public String Text;
        public Integer ApplicationUserId;
        public Integer PersonInfoId;

        @Override
        public String Name() {
            return Models.convertFIOToName(PersonInfo);
        }
    }

    /**
     * Пациент
     */
    public static class Patient extends NamedModel {

        public PersonInfo PersonInfo;
        public Integer MedicalCardNumber;
        public Date DateOfBirth;
        public Boolean IsMen;
        public Integer ApplicationUserId;
        public Integer PersonInfoId;
        public List<Visit> Visits = new ArrayList<Visit>();
        public List<Order> Orders = new ArrayList<Order>();

        @Override
        public String Name() {
            return Models.convertFIOToName(PersonInfo);
        }
    }

    /**
     * Посешение
     */
    public static class Visit extends NamedModel {

        public Doctor Doctor;
        public java.util.Date Date;
        public String Annotation;
        public Integer PatientId;
        public Boolean IsClosed;
        public Integer DoctorId;
        public List<Procedure> Procedures = new ArrayList<Procedure>();

        @Override
        public String Name() {
            return  "#"+Id+" - "+(Date!=null ? df.format(Date) : "null");
        }
    }

    /**
     * Наряд-заказ
     */
    public static class Order  extends  NamedModel{

        public DentalTechnican DentalTechnican;
        public Doctor Doctor;
        public ClinicInfo ClinicInfo;
        public java.util.Date Date;
        public java.util.Date FinishDate;
        public Boolean IsClosed;
        public Boolean IsFinished;
        public String Annotation;
        public Integer PatientId;
        public Integer DoctorId;
        public Integer DentalTechnicanId;
        public Integer ClinicInfoId;
        public float Cost;

        public List<Tooth> Teeth = new ArrayList<Tooth>();

        @Override
        public String Name() {
            return "#"+Id+" - "+(Date != null ? df.format(Date) :"");
        }

    }

    /**
     * Элемент наряд заказа - работа по зубу
     */
    public static class Tooth extends NamedModel
    {

        public Integer ToothNo;
        public Integer ProcedureId;
        public TechnicanProcedure Procedure;
        public Integer OrderId;


        @Override
        public String Name() {
            return "Зуб "+ToothNo+": "+Procedure.Name;
        }

        @Override
        public float Cost()
        {
            return Procedure.Cost;
        }
    }

    /**
     * То, что может изготовить техние
     */
    public static class TechnicanProcedure extends NamedModel
    {
        public String Name;
        public Integer Cost;

        @Override
        public String Name() {
            return Name;
        }

        @Override
        public float Cost(){
            return Cost;
        }
    }

    // РЕГИСТРАЦИЯ

    public static class PatientRegistrationViewModel {

        public Date DateOfBirth;
        public Integer MedicalCardNumber;
        public Boolean IsMen;
        public String Email;
        public String Password;
        public String ConfirmPassword;
        public String Name;
        public String Surname;
        public String Middlename;

    }

    public static class DoctorRegistrationViewModel {

        public String Email;
        public String Password;
        public String ConfirmPassword;
        public String Name;
        public String Surname;
        public String Middlename;
        public String Image;
        public String Description;

    }

    public static class TechnicanRegistrationViewModel {

        public String Email;
        public String Password;
        public String ConfirmPassword;
        public String Name;
        public String Surname;
        public String Middlename;

    }
}