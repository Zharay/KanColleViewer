function Main
{
	begin
	{
		$output = '../resources/Version.xml'
		$resources = '../resources/translations'
		$projectPath = '../Grabacr07.KanColleViewer'
		$baseTransURL = 'http://yuyuvn.github.io/KanColleViewer/resources/translations'
		$update_url = 'https://github.com/yuyuvn/KanColleViewer/releases/tag/'
		$xdoc = new-object System.Xml.XmlDocument
		
		if (-not(Test-Path $resources))
        {
            throw 'Script detected as locate in invalid path exception!! Make sure exist in <KanColleViewer repository root>\tools-release\'
        }
		
		[xml]$data = @"
<?xml version="1.0" encoding="utf-8"?>
<Versions />
"@
	}
	
	process
	{
		$app_version = (Get-Content $(Join-Path $projectPath 'Properties/AssemblyInfo.cs') | Select-String -Pattern '\("[0-9]+\.[0-9]+\.[0-9]+\.([0-9]+)"\)' -list).tostring().split('"')[1]
		
		$av = $app_version.split('.')
		$app_version_tag = "v" + $av[0] + "." + $av[1] +"." + $av[2] + "r" + $av[3]
		
		$settings = [xml] (get-content $(Join-Path $projectPath 'Properties/Settings.settings'))
		$ns = New-Object System.Xml.XmlNamespaceManager($settings.NameTable)
		$ns.AddNamespace("ns", $settings["SettingsFile"].Attributes["xmlns"].Value)
		$url = $baseTransURL
		
		$t = $data.CreateElement("Item")
		$tc = $data.CreateElement("Name")
		$tc.set_InnerText('App')
		$t.AppendChild($tc)
		$tc = $data.CreateElement("Version")
		$tc.set_InnerText($app_version)
		$t.AppendChild($tc)
		$tc = $data.CreateElement("URL")
		$tc.set_InnerText($update_url + $app_version_tag)
		$t.AppendChild($tc)
		$data["Versions"].AppendChild($t)
			
		Get-ChildItem -Path $resources -Filter '*.xml' | % {
			$version = ([xml] (get-content $_.FullName))[$_.BaseName].GetAttribute("Version")
			$t = $data.CreateElement("Item")
			$tc = $data.CreateElement("Name")
			$tc.set_InnerText($_.BaseName)
			$t.AppendChild($tc)
			$tc = $data.CreateElement("Version")
			$tc.set_InnerText($version)
			$t.AppendChild($tc)
			$tc = $data.CreateElement("URL")
			$tc.set_InnerText($url + "/" + $_.Name)
			$t.AppendChild($tc)
			$data["Versions"].AppendChild($t)
		}
	}
	
	end
	{
		If (Test-Path $output) {
			Remove-item -Path $output
		}
		New-Item -ItemType file -Path $output
		$data.Save((resolve-path $output).tostring())
	}
}

Main