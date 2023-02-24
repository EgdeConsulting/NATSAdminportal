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

function NavItem({
  navSize,
  icon,
  title,
  route,
  width,
}: {
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
      alignItems={navSize == 200 ? "center" : "flex-start"}
    >
      <Menu placement="right">
        <Link
          as={RouteLink}
          to={route}
          _hover={{
            textDecor: "none",
            backgroundColor:
              colorMode === "dark" ? "whiteAlpha.300" : "gray.200",
          }}
          p={2}
          borderRadius={8}
        >
          <MenuButton w={width}>
            <Flex>
              <Icon as={icon} fontSize="xl" marginTop={1} marginLeft={0.5} />
              <Text
                ml={5}
                align="center"
                display={navSize == 200 ? "none" : "flex-start"}
              >
                {title}
              </Text>
            </Flex>
          </MenuButton>
        </Link>
      </Menu>
    </Flex>
  );
}
export { NavItem };
