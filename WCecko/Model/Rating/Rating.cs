using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WCecko.Model.Rating;

public class Rating : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public int PlaceId { get; set; }

    [Indexed, NotNull]
    public string CreatedBy { get; set; } = "";

    private int _stars;

    [NotNull]
    public int Stars
    {
        get => _stars;
        set
        {
            if (_stars == value)
                return;
            _stars = value;
            OnPropertyChanged();
        }
    }

    private string _comment = string.Empty;

    [NotNull]
    public string Comment
    {
        get => _comment;
        set
        {
            if (_comment == value)
                return;
            _comment = value;
            OnPropertyChanged();
        }
    }

    [Ignore]
    public bool ModifyPermission { get; set; } = true;  // TODO: set to false later


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
