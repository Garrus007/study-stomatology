package com.example.stomatologyclient.activity;

import android.content.Context;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.example.stomatologyclient.R;
import com.example.stomatologyclient.adapters.OnListInteractListener;
import com.example.stomatologyclient.adapters.UniversalListAdapter;
import com.example.stomatologyclient.api.API;
import com.example.stomatologyclient.api.Models;
import com.example.stomatologyclient.api.RetrofitFactory;
import com.example.stomatologyclient.auth.StomatologyAccountManager;
import com.example.stomatologyclient.dialogs.ProcedureSelectDialog;

import java.text.SimpleDateFormat;
import java.util.List;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import trikita.knork.Knork;

public class VisitActivity extends AbstractNavigationActivity implements
        OnListInteractListener,
        ProcedureSelectDialog.OnItemeSelectedListener
{

    @Knork.Id(R.id.list)
    RecyclerView recyclerView;
    @Knork.Id(R.id.visit_doctor)
    TextView doctor;
    @Knork.Id(R.id.visit_date)
    TextView date;
    @Knork.Id(R.id.button_close)
    Button close_button;
    @Knork.Id(R.id.button_save)
    Button save_button;
    @Knork.Id(R.id.visit_annotations)
    EditText annotations;
    @Knork.Id(R.id.fab)
    FloatingActionButton fab;

    private UniversalListAdapter adapter;
    private List<Models.Procedure> procedures;

    private Retrofit retrofit;
    private API api;

    Models.Visit visit_model;
    boolean is_doctor;

    SimpleDateFormat format = new SimpleDateFormat("dd.MM.yyyy");

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_visit);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        Knork.inject(getWindow().getDecorView(), this);
        final ProcedureSelectDialog.OnItemeSelectedListener listener = this;


        //Добавление нового посещения
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //Показываем диалог выбора процедуры
                ProcedureSelectDialog dlg = new ProcedureSelectDialog(listener);
                android.support.v4.app.FragmentManager fm = getSupportFragmentManager();
                dlg.show(fm, "select_procedure");
            }
        });

        //Проверка ролей
        is_doctor = StomatologyAccountManager.getRole(this)==StomatologyAccountManager.ROLE_DOCTOR;
        if(is_doctor)
            fab.setVisibility(View.VISIBLE);
        else
            fab.setVisibility(View.GONE);

        retrofit = RetrofitFactory.GetRetrofit(StomatologyAccountManager.getAuthToken(this));
        api = retrofit.create(API.class);


        update_visit();
    }

    private void update_visit()
    {
        final Context context = this;
        final OnListInteractListener listner = this;

        final Call<Models.Visit> visit = api.getVisit(id);
        visit.enqueue(new Callback<Models.Visit>() {
            @Override
            public void onResponse(Call<Models.Visit> call, Response<Models.Visit> response) {
             if(response.code()==200) {
                 visit_model = response.body();

                 if (visit_model.Doctor != null)
                     doctor.setText(visit_model.Doctor.Name());

                 if (visit_model.Date != null)
                     date.setText(format.format(visit_model.Date));

                 annotations.setText(visit_model.Annotation);

                 //Процедуры
                 procedures = visit_model.Procedures;
                 adapter = new UniversalListAdapter(context, procedures, false, false, false, is_doctor && !visit_model.IsClosed);
                 adapter.setOnListInteractListenr(listner);
                 recyclerView.setAdapter(adapter);
                 recyclerView.setLayoutManager(new LinearLayoutManager(context));


                 //Делаем элементы неактивынми, если посещение завершено
                 if (visit_model.IsClosed)
                     set_closed();
             }
            }

            @Override
            public void onFailure(Call<Models.Visit> call, Throwable t) {
                Toast.makeText(context,"Не удалось загрузить посещение",Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void set_closed()
    {
        annotations.setEnabled(false);
        fab.setEnabled(false);
        close_button.setEnabled(false);
        close_button.setText("Посещение закрыто");
        save_button.setEnabled(false);
    }

    @Override
    public void OnItemClick(int id) {

    }

    //Удаление процедурыы
    @Override
    public void OnRemoveClick(final int id) {
        Call<ResponseBody> resp = api.removeVisitProcedure(this.id, id);
        final Context context = this;
        resp.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if(response.code()==200)
                {
                    adapter.remove(id);
                }
                else
                    Toast.makeText(context,"Не удалось удалить процедуру",Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(context,"Не удалось удалить процедуру",Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public void OnEditClick(int id) {

    }

    //Сохранение
    @Knork.On({Knork.CLICK + R.id.button_save})
    public void save_click(View v)
    {

        visit_model.Annotation = annotations.getText().toString();

        final Context context = this;

        final Call<ResponseBody> resp = api.postVisit(visit_model);
        resp.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if(response.code()==200)
                    Toast.makeText(context,"Сохранено",Toast.LENGTH_SHORT).show();
                else
                    Toast.makeText(context,"Не удалось сохранить посещение",Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(context,"Не удалось сохранить посещение",Toast.LENGTH_SHORT).show();
            }
        });
    }

    //Закрытие
    @Knork.On({Knork.CLICK + R.id.button_close})
    public void close_click(View v)
    {
        final Context context = this;
        final Call<ResponseBody> resp = api.closeVisit(visit_model.Id);
        resp.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if(response.code()==200)
                    set_closed();
                else
                    Toast.makeText(context,"Не удалось закрыть посещение",Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(context,"Не удалось закрыть посещение",Toast.LENGTH_SHORT).show();
            }
        });
    }


    //Выбрана процедура для добавления в посещение
    @Override
    public void itemSelected(final Object item)
    {
        final Models.Procedure procedure = (Models.Procedure)item;
        final Context context = this;

        Call<ResponseBody> resp = api.addVisitProcedure(id, procedure.Id);
        resp.enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if(response.code()==200)
                {
                    procedures.add(procedure);
                    adapter.notifyItemInserted(procedures.size());
                }
                else
                    Toast.makeText(context,"Не удалось добавить процедуру",Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(context,"Не удалось добавить процедуру",Toast.LENGTH_SHORT).show();
            }
        });
    }
}
