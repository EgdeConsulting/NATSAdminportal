import {
  Menu,
  MenuButton,
  MenuList,
  MenuGroup,
  MenuItem,
  MenuDivider,
  Button,
  Avatar,
} from "@chakra-ui/react";
import { useState } from "react";

function AccountMenu() {
  const [activeAccount, setActiveAccount] = useState("Daniel");
  const [accounts, setAccounts] = useState(["Tobias", "Simen"]);

  function changeAccount(index: number) {
    let newActiveAccount = accounts[index];

    const queryString = "username=" + newActiveAccount;
    fetch("/api/updateUserAccount?" + queryString, {
      method: "POST",
    });

    setAccounts(
      accounts
        .filter((item) => item !== accounts[index])
        .concat([activeAccount])
    );
    setActiveAccount(newActiveAccount);
  }

  return (
    <Menu>
      <MenuButton
        width={"54px"}
        margin={2}
        size={"md"}
        as={Button}
        colorScheme="gray"
      >
        <Avatar
          marginLeft={"-4.1px"}
          size={"sm"}
          src={"https://bit.ly/broken-link"}
        />
      </MenuButton>
      <MenuList>
        <MenuGroup title="Logged in as">
          <MenuItem closeOnSelect={false}>{activeAccount}</MenuItem>
        </MenuGroup>
        <MenuDivider />
        <MenuGroup title="Switch account">
          <MenuItem closeOnSelect={false} onClick={() => changeAccount(0)}>
            {accounts[0]}
          </MenuItem>
          <MenuItem closeOnSelect={false} onClick={() => changeAccount(1)}>
            {accounts[1]}
          </MenuItem>
        </MenuGroup>
      </MenuList>
    </Menu>
  );
}

export { AccountMenu };
