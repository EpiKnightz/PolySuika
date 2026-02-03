public class UIChangeTextOnStringEvent : UIChangeTextOnEvent<string>
{
    protected override void OnTriggerChange(string newText)
    {
        if (newText != string.Empty)
        {
            if (!TextComponent.enabled)
            {
                TextComponent.enabled = true;
            }
            // TODO: Hax - for some reason text component just refuse to update
            gameObject.SetActive(false);
            TextComponent.text = newText;
            gameObject.SetActive(true);
        }
        else
        {
            TextComponent.enabled = false;
        }
    }
}
