using System;
using System.Reflection;

public abstract class Model : Notifier
{
    protected string _modelName;
    public bool IsModelNameValide()
    {
        return !HasEvent(_modelName);
    }
    public void Inject(object obj)
    {
        FieldInfo[] fields = obj.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            FieldInfo myField = this.GetType().GetField(field.Name);
            if (null != myField)
            {
                myField.SetValue(this, field.GetValue(obj));
            }
        }
        PropertyInfo[] properties = obj.GetType().GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            PropertyInfo myPro = this.GetType().GetProperty(property.Name);
            if (null != myPro && myPro.CanWrite)
            {
                myPro.SetValue(this, property.GetGetMethod().Invoke(obj, null), null);
            }
        }

    }
    public void Refresh(Enum attribute, params object[] e)
    {
        RasiseEvent(attribute.ToString(), e);
    }
    public virtual void Destroy()
    {
    }
    public void AddEventHandlerEx(Enum Attribute, Notifier.StandardDelegate fun)
    {
        AddEventHandler(Attribute.ToString(), fun);
    }
    public void RemoveEventHandleEx(Enum Attribute, Notifier.StandardDelegate fun)
    {
        RemoveEventHandler(Attribute.ToString(), fun);
    }
}

