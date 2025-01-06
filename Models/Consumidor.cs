public class Consumidor
{
    public int id_consumidor { get; set; }
    public string? nm_consumidor { get; set; }
    public string? nr_documento { get; set; }
    public int? id_tipo_documento { get; set; }
    public string? ds_email { get; set; }
    public string? nr_celular { get; set; }
    public char fl_crm { get; set; } // Armazena o valor verdadeiro/falso internamente
    public char fl_sms { get; set; }
    public char fl_email { get; set; }
    public string? nr_cep { get; set; }
    public string? ds_endereco { get; set; }
    public string? ds_bairro { get; set; }
    public string? nm_cidade { get; set; }
    public string? sg_uf { get; set; }
    public int? nr_dia_aniversario { get; set; }
    public int? nr_mes_aniversario { get; set; }

}
