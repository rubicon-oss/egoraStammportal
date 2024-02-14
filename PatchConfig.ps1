$FilePath= $args[0]
$conf = Get-Content -Path $FilePath

$conf = $conf -replace 'authentication mode="Windows"', 'authentication mode="Forms"';

$conf = $conf -replace '<!--PlaceHolderForms-->', @"
<forms name="StammportalFormsAuth" loginUrl="winauth/login.aspx" />
      <!-- <forms name="StammportalFormsAuth" loginUrl="formsauth/login.aspx" /> -->
"@
;

$conf = $conf -replace '<!--PlaceHolderAuthorization-->', @"
<authorization>
      <deny users="?"/>
      <allow users="*"/>
    </authorization>    
"@
;

$conf = $conf -replace '<!--PlaceHolderMachineKey-->', '<machineKey decryptionKey="E37297BECE1BFC6088BF3DF417F64D5F509A363CAF2452F4" validationKey="E2A8C42E3B90F80B447331F0EFC8EC632EAFB316484A899405A7BDA2B3409AA6186ED14576175FF259BB96FF656BCEF93C1DF9E4C8CEDFE1B7ADE8E73937EAB7" />';
  
$conf = $conf -replace '<!--PlaceHolderLocationAuth-->', @"
  <location path="winauth">
  <!-- <location path="formsauth"> -->
    <system.web>
      <authorization>
        <allow users="*"/>
        <allow users="?"/>
      </authorization>
    </system.web>
  </location>
"@ 
;

Set-Content -Path $FilePath -Value $conf


