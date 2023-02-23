import {
  Flex,
  Menu,
  MenuButton,
  Link,
  Icon,
  Text,
  useColorMode,
} from "@chakra-ui/react";
import { IconType } from "react-icons";
import { Link as RouteLink } from "react-router-dom";

function NavItem(props: {
  navSize: number;
  icon: IconType;
  title: string;
  route: string;
  width: number;
}) {
  const { colorMode, toggleColorMode } = useColorMode();
  return (
    <Flex
      mt={30}
      flexDir="column"
      alignItems={props.navSize == 200 ? "center" : "flex-start"}
    >
      <Menu placement="right">
        <Link
          as={RouteLink}
          to={props.route}
          _hover={{
            textDecor: "none",
            backgroundColor:
              colorMode === "dark" ? "whiteAlpha.300" : "gray.200",
          }}
          p={2}
          borderRadius={8}
        >
          <MenuButton w={props.width}>
            <Flex>
              <Icon
                as={props.icon}
                fontSize="xl"
                marginTop={1}
                marginLeft={0.5}
              />
              <Text
                ml={5}
                align="center"
                display={props.navSize == 200 ? "none" : "flex-start"}
              >
                {props.title}
              </Text>
            </Flex>
          </MenuButton>
        </Link>
      </Menu>
    </Flex>
  );
}
export { NavItem };
