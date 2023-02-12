import { Flex, Menu, MenuButton, Link, Icon, Text } from "@chakra-ui/react";
import { IconType } from "react-icons";
import { Link as RouteLink } from "react-router-dom";
function NavItem({
  navSize,
  icon,
  title,
  route,
}: {
  navSize: number;
  icon: IconType;
  title: string;
  route: string;
}) {
  return (
    <Flex
      mt={30}
      flexDir="column"
      w="100%"
      alignItems={navSize == 200 ? "center" : "flex-start"}
    >
      <Menu placement="right">
        <Link
          as={RouteLink}
          to={route}
          _hover={{ textDecor: "none", backgroundColor: "gray.500" }}
          p={2}
          borderRadius={8}
        >
          <MenuButton w="100%">
            <Flex>
              <Icon as={icon} fontSize="xl" marginTop={1} />
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
